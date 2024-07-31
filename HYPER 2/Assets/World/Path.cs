using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Assertions;
using static Helpers.General;

[Serializable]
public class Branch {
    public Direction direction;
    public int length = 0;
    public int startIndex;
    public Tile start;
    public int endIndex;
    public Tile end;
    [SerializeReference] public List<Branch> branches = new List<Branch>();

    public Branch(Tile start, Direction direction, int length, Tile end) {
        this.length = length;
        this.direction = direction;
        this.start = start;
        startIndex = start.Index;
        this.end = end;
        endIndex = end.Index;
    }

    public Branch(Direction direction, Tile end, int length, Branch parent) {
        this.direction = direction;
        this.end = end;
        endIndex = end.Index;
        parent.SetBranch(this);
        start = parent.end;
        startIndex = start.Index;

        this.length = length;
    }

    public Option<Branch> Next() {
        if (branches.Count == 0) {
            return Option<Branch>.None();
        }else {
            return new Option<Branch>(branches[0]);
        }
    }

    public void SetBranch(Branch branch) {
        branches.Add(branch);
    }
}

public class Path : MonoBehaviour
{

    public enum Message {
        ALL_GOOD,
        IMPROPER_PLACEMENT
    }

    [SerializeField] private Interval spawnInterval;
    [SerializeField] private Interval spawnInterval2;

    private World world;
    private WorldGenerator wg;
    private List<Neighbor> neighbors1 = new List<Neighbor>();
    private List<Neighbor> neighbors2 = new List<Neighbor>();
    private Direction mainDirection = Direction.UNKNOWN;
    Direction perpendicular, perpendicular2;// to the main
    private Direction currentDirection = Direction.UNKNOWN;
    private Branch root1;
    public Branch Root1 {
        get { return root1; }
    }
    private int closestCornerDist1;
    private float closestCornerPercent;
    private Option<Branch> currentBranch1 = Option<Branch>.None();
    private Branch root2;
    public Branch Root2 {
        get { return root2; }
    }


    private int amount = 0;
    private List<Neighbor> buffer = new List<Neighbor>();

    //private Manager manager;
    private IRound round;

    private int index = 0;

    public void DestroyPath() {
        foreach (var nb in neighbors1) {
            nb.Tile.Disable();
        }
        Destroy(gameObject);
    }

    private void CalculateCornerToIndexDist(Tile start) {
        var (cornerIndex1, cornerIndex2) = wg.GetCornersBetweenIndex(start.Index, start.Layer);
        var diff1 = Mathf.Abs(start.Index-cornerIndex1);
        var diff2 = Mathf.Abs(start.Index-cornerIndex2);
        closestCornerDist1 = diff1 > diff2 ? diff1 : diff2;
        var length = Mathf.Abs(cornerIndex2-cornerIndex1);
        closestCornerPercent = closestCornerDist1/length;
    }

    private void GetMainDirFromLocation(Location location) {
        switch (location.Type) {
        case LocationType.LEFT_BOTTOM:
            mainDirection = FiftyFifty(Direction.RIGHT, Direction.UP);
            break;
        case LocationType.FIRST:
        case LocationType.BOTTOM:
            mainDirection = Direction.UP;
            break;
        case LocationType.BOTTOM_RIGHT:
            mainDirection = FiftyFifty(Direction.LEFT, Direction.UP);
            break;
        case LocationType.RIGHT:
            mainDirection = Direction.LEFT;
            break;
        case LocationType.RIGHT_TOP:
            mainDirection = FiftyFifty(Direction.LEFT, Direction.DOWN);
            break;
        case LocationType.TOP:
            mainDirection = Direction.DOWN;
            break;
        case LocationType.TOP_LEFT:
            mainDirection = FiftyFifty(Direction.RIGHT, Direction.DOWN);
            break;
        case LocationType.LEFT:
            mainDirection = Direction.RIGHT;
            break;
        }
    }

    public void CStart(Manager manager, Tile start) {
        //this.manager = manager;
        world = manager.World;
        wg = world.WG;
        start.Enable();
        GetMainDirFromLocation(start.Location);
        neighbors1.Add(new Neighbor(mainDirection, start));
        
        CalculateCornerToIndexDist(start);

        perpendicular = (Direction)Num.Wrap((int)mainDirection+1, 0, 3);
        perpendicular2 = (Direction)Num.Wrap((int)mainDirection+3, 0, 3);

        extend();

        Save.rData.AddRoot1(root1);
        index = Save.rData.Roots.Count-1;
    }

    public void CStart(Manager manager, Branch root) {
        world = manager.World;
        wg = world.WG;
        root1 = root;
        GetMainDirFromLocation(wg[root.startIndex].Location);
        perpendicular = (Direction)Num.Wrap((int)mainDirection+1, 0, 3);
        perpendicular2 = (Direction)Num.Wrap((int)mainDirection+3, 0, 3);
        CalculateCornerToIndexDist(wg[root.startIndex]);
        RecreatePath(ref neighbors1, root);
    }

    public void Widen() {
        var sideDirection =  Mathf.Abs(neighbors1[0].Location.CornersRange.a) < Mathf.Abs(neighbors1[0].Location.CornersRange.b) ? perpendicular : perpendicular2; 
        // if (neighbors1[1].Direction == Direction.UP | neighbors1[1].Direction == Direction.DOWN) {
        //     sideDirection = sideDirection == perpendicular2 ? perpendicular : perpendicular2;
        // }

        Option<Neighbor> start = Option<Neighbor>.None();
        var nbs = neighbors1[0].Tile.GetNeighbors();
        foreach (var nb in nbs) {
            if (nb.Direction == sideDirection) {
                start = new Option<Neighbor>(nb);
                break;
            }
        }
        Assert.IsFalse(start.IsNone());
        start.GetAndThen(s => {
            s.Direction = mainDirection;
            s.Tile.Enable();
            neighbors2.Add(s);
        });

        //var color = new Color(1f,0f,1f);

        var currentTile = neighbors2[neighbors2.Count-1].Tile;
        var found = false;
        while (validForExtending(currentTile)) {
            if (currentTile.Distance < 9f) {
                Debug.Log("breakpoint");
            }
            nbs = currentTile.GetNeighbors();
            foreach (var nb in nbs) {
                if (nb.Direction == mainDirection) {
                    nb.Tile.Enable();
                    currentTile = nb.Tile;
                    neighbors2.Add(nb);
                    found = true;
                    break;
                }
            }
            if (!found) {
                nbs[0].Tile.Enable();
                neighbors2.Add(nbs[0]);
                currentTile = nbs[0].Tile;
            }
            found = false;
        }

        root2 = CreateBranches(neighbors2);
    }

    private Branch CreateBranches(List<Neighbor> neighbors) {
        Branch root = null;
        var current = Option<Branch>.None();
        currentDirection = mainDirection;

        int counter = 0;
        var prevStart = neighbors[0];
        for (int i = 1; i < neighbors.Count; i++) {
            if (i+1 > neighbors.Count-1) {
                continue;
            }
            counter++;
            if (neighbors[i+1].Direction != currentDirection && neighbors[i].Direction == currentDirection) {
                if (root == null) {
                    root = new Branch(prevStart.Tile, currentDirection, counter, neighbors[i].Tile);
                    current = new Option<Branch>(root);
                }else {
                    current.GetAndThen(c => {
                        current = new Option<Branch>( new Branch(currentDirection, neighbors[i].Tile, counter, c) );
                    });
                }
                prevStart = neighbors[i];

                currentDirection = neighbors[i+1].Direction;
                counter = 0;
            }
        }
        currentDirection = mainDirection;

        var last = neighbors[neighbors.Count-1];
        if (root == null) {
            root = new Branch(prevStart.Tile, currentDirection, counter+1, last.Tile);
            current = new Option<Branch>(root);
        }else {
            current.GetAndThen(c => {
                current = new Option<Branch>( new Branch(last.Direction, last.Tile, counter+1, c) );
            });
        }

        return root;
    }

    public Interval[] StartSpawning(IRound round) {
        this.round = round;
        spawnInterval.Enable();
        if (neighbors2.Count != 0) {
            spawnInterval2.time = 0.3f;
            spawnInterval2.Enable();
            return new Interval[] {
                spawnInterval, spawnInterval2
            };
        }
        return new Interval[] {
            spawnInterval
        };
    }

    public void StopSpawning() {
        spawnInterval.Disable();
        spawnInterval2.Disable();
    }

    public void SpawnBoss() {
        round.Spawn(root1, mainDirection, neighbors1[0].Tile.transform.position+Vector3.up*1.5f, true);
    }

    public void IntervalSpawnEnemy() {
        round.Spawn(root1, mainDirection, neighbors1[0].Tile.transform.position+Vector3.up*1.5f);
    }
    public void IntervalSpawnEnemy2() {
        round.Spawn(root2, mainDirection, neighbors2[0].Tile.transform.position+Vector3.up*1.5f);
    }

    private void extend() {
        //Color color = Color.red;
        int noFinds = 0;
        while (validForExtending(neighbors1[neighbors1.Count-1].Tile)) {
            if (buffer.Count == 0)
                buffer = neighbors1[neighbors1.Count-1].Tile.GetNeighbors();
            if (amount > 0) {
                var leave = false;
                foreach (var nb in buffer) {
                    if (nb.Direction == currentDirection) {
                        if (EnableNeighbor(nb) != Message.ALL_GOOD) {
                            amount = 0;
                            //color = Color.gray;
                            leave = true;
                            break;
                        }
                        amount--;
                        leave = true;
                        break;
                    }
                }
                if (leave) {
                    continue;
                }else {
                    Debug.Log("Didn't find at: "+neighbors1[neighbors1.Count-1].Tile.Index+" with direction "+currentDirection);
                    if (noFinds > 2) {
                        break;
                    }
                    noFinds++;
                    amount = 0;
                    DisableNeighbor(neighbors1[neighbors1.Count-1]);
                    continue;
                }
            }else {
                //color = Color.red;
            }
            // if (buffer.Count == 0) {
            //     break;
            // }
            var closestToTower = buffer[0];
            if (wg.IndexBelongsTo(neighbors1[neighbors1.Count-1].Tile.Index, 1) | 
                ((int)neighbors1[neighbors1.Count-1].Location.Type % 2 == 0 && neighbors1[neighbors1.Count-1].Location.Type != LocationType.FIRST)) {
                GetDeviatedNeighbor().GetAndThen(n => { closestToTower = n; });
            }

            if (currentDirection == closestToTower.Direction) {
                EnableNeighbor(closestToTower);
                continue;
            }
            //color = new Color(0, Random.Range(0.5f, 1f), Random.Range(0f, 1f));
            EnableNeighbor(closestToTower);
            currentDirection = closestToTower.Direction;

            if (currentDirection != mainDirection) {
                var range = closestToTower.Location.CornersRange;
                //Debug.Log("Range: "+range.a+", "+range.b);
                range.a = Mathf.Abs(range.a);
                range.b = Mathf.Abs(range.b);
                
                if (range.a > range.b) {
                    SetAmount(range.a);
                }else {
                    SetAmount(range.b);
                }
            }
        }

        root1 = CreateBranches(neighbors1);
    }

    private void RecreatePath(ref List<Neighbor> neighbors, Branch root) {
        Option<Branch> curr = new Option<Branch>(root);
        {
            var currBranch = curr.Get();
            var start = wg[currBranch.startIndex-1];
            currBranch.start = start;
            currBranch.end = wg[currBranch.endIndex-1];
            start.Enable();
            neighbors.Add(new Neighbor(mainDirection, start));
        }
        while(curr.IsSome()) {
            var currBranch = curr.Get();
            currBranch.start = wg[currBranch.startIndex-1];
            currBranch.end = wg[currBranch.endIndex-1];

            var nb = neighbors[neighbors.Count-1];
            for (int i = 0; i < currBranch.length; i++) {
                var hypotheticalNB = nb.Tile.GetNeighbor(currBranch.direction);
                Assert.IsFalse(hypotheticalNB.IsNone(), "recreating path error: no neighbor found trying to go "+currBranch.direction+" at "+nb.Tile.Index);
                nb = hypotheticalNB.Get();
                nb.Tile.Enable();
                neighbors.Add(nb);
            }

            curr = currBranch.Next();
        } 
    }

    public Option<Neighbor> GetDeviatedNeighbor() {
        Debug.Log("Deviation at "+neighbors1[neighbors1.Count-1].Tile.Index+" with "+neighbors1[neighbors1.Count-1].Location.Type);
        Option<Neighbor> a = Option<Neighbor>.None(), b = Option<Neighbor>.None();
            
        foreach (var nb in buffer) {
            if (nb.Direction == perpendicular) {
                a = new Option<Neighbor>(nb);
            }else if (nb.Direction == perpendicular2) {
                b = new Option<Neighbor>(nb);
            }
        }

        if (a.IsNone() && b.IsSome()) {
            return b;
        }else if (b.IsNone() && a.IsSome()) {
            return a;
        }else if (b.IsSome() && a.IsSome()) {
            return a.Get().Tile.Distance > b.Get().Tile.Distance ? b : a;
        }
        return Option<Neighbor>.None();
    }

    private void SetAmount(int value) {
        var min = 3;

        var max = Mathf.FloorToInt(value/2f)-1;
        if (max > 6) {
            max = 6;
        }
        if (min > max) {
            min = max;
        }
        // if (max == 1) {
        //     Debug.Log("breakpoint");
        // }

        amount = Seed.IntRange(min, max);
    }

    private void debugExtend() {
        buffer = neighbors1[neighbors1.Count-1].Tile.GetNeighbors();
        foreach (var nb in buffer) {
            nb.Tile.Enable();
            neighbors1.Add(nb);
        }
    }

    private Message EnableNeighbor(Neighbor nb) {
        //Debug.Log("Tile index: "+nb.Tile.Index);
        //Debug.Log("Though to be "+nb.Direction+" to: "+neighbors1[neighbors1.Count-1].Tile.Index);
 
        if (!validForExtending(nb.Tile)) {
            goto All_good;
        }

        var localNeighbors = nb.Tile.GetNeighbors();
        if (localNeighbors.Count >= 3) {
            goto All_good;
        }
        //Debug.Log("At "+nb.Tile.Index+", this was no good");
        //Debug.Log("Neighbors it had: ");
        // foreach (var nbr in localNeighbors) {
        //     //Debug.Log("  "+nbr.Tile.Index+" | "+nbr.Location.Type+" / "+nbr.Direction);
        // }
        buffer.Remove(nb);
        return Message.IMPROPER_PLACEMENT;
    All_good:
        nb.Tile.Enable();
        neighbors1.Add(nb);
        buffer.Clear();
        return Message.ALL_GOOD;
    }
    // private Message EnableNeighbor(Neighbor nb, Color color) {
    //     if (!validForExtending(nb.Tile)) {
    //         goto All_good;
    //     }

    //     var localNeighbors = nb.Tile.GetNeighbors();
    //     if (localNeighbors.Count >= 3) {
    //         goto All_good;
    //     }
    //     buffer.Remove(nb);
    //     return Message.IMPROPER_PLACEMENT;
    // All_good:
    //     nb.Tile.Enable(color);
    //     neighbors1.Add(nb);
    //     buffer.Clear();
    //     return Message.ALL_GOOD;
    // }

    private void DisableNeighbor(Neighbor nb) {
        nb.Tile.Disable();
        neighbors1.Remove(nb);
    }

    private bool validForExtending(Tile tile) {
        if (tile.Layer == 1) {
            int location = (int)wg.GetLocationOfIndex(tile.Index, tile.Layer).Type;
            if (location == 0) {
                return false;
            }
            return location % 2 == 0;
        }
        return true;
    }

    public void RepositionPath() {
        RepositionRoot(neighbors1, root1);
        Save.rData.Roots[index] = root1;
        RepositionRoot(neighbors2, root2);
    }

    private void RepositionRoot(List<Neighbor> neighbors, Branch root) {
        if (neighbors.Count == 0) { return; }
        if (root == null) { return; }

        var nbs = neighbors[0].Tile.GetNeighbors();
        var opp = Dir.Opposite(mainDirection);
        Option<Neighbor> nbFound = Option<Neighbor>.None();

        foreach (var nb in nbs) {
            if (nb.Direction == opp) {
                nb.Tile.Enable();
                neighbors.Insert(0, nb);
                nbFound = new Option<Neighbor>(nb);
                root.start = nb.Tile;
                break;
            }
        }
        if (nbFound.IsSome()) {
            RepositionRoot(neighbors, root);
        }
    }
}   
