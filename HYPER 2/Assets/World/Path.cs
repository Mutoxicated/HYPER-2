using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Assertions;
using static Helpers.General;

public class Branch {
    public Direction direction;
    public Vector3 start;
    public Vector3 end;
    public List<Branch> branches = new List<Branch>();

    public Branch(Vector3 start, Direction direction, Vector3 end) {
        this.direction = direction;
        this.start = start;
        this.end = end;
    }

    public Branch(Direction direction, Vector3 end, Branch parent) {
        this.direction = direction;
        this.end = end;
        parent.SetBranch(this);
        start = parent.end;
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

    private World world;
    private WorldGenerator wg;
    private List<Neighbor> neighbors1 = new List<Neighbor>();
    private List<Neighbor> neighbors2 = new List<Neighbor>();
    private Direction mainDirection = Direction.UNKNOWN;
    private Direction currentDirection = Direction.UNKNOWN;
    private Branch root1;
    private Option<Branch> currentBranch1 = Option<Branch>.None();
    private Branch root2;

    private int amount = 0;
    private List<Neighbor> buffer = new List<Neighbor>();

    //private Manager manager;
    private IRound round;

    public void DestroyPath() {
        foreach (var nb in neighbors1) {
            nb.Tile.Disable();
        }
        Destroy(gameObject);
    }

    public void CStart(Manager manager, Tile start) {
        //this.manager = manager;
        world = manager.World;
        wg = world.WG;
        start.Enable();
        var location = wg.GetLocationOfIndex(start.Index, start.Layer);
        neighbors1.Add(new Neighbor(Direction.UNKNOWN, start, location));

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

        extend();

        var dir = wg.Tower.transform.position-neighbors1[neighbors1.Count-1].Tile.transform.position;
        if (dir.y > 0f) {
            dir.y = 0f;
        }
        var direction = Dir.FromVec(dir);
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA "+direction);
        if (currentBranch1.Get().direction != direction) {
            AddBranch(neighbors1, direction, 1);
            currentBranch1.Get().end += dir.normalized*2f;
        }
    }

    public void Widen() {
        var offset = Mathf.Abs(neighbors1[0].Location.CornersRange.a) > Mathf.Abs(neighbors1[0].Location.CornersRange.b) ? 3 : 1;
        var sideDirection = (Direction)Num.Wrap((int)mainDirection+offset, 0, 3);
        var deviatedSideDirection = (Direction)Num.Wrap((int)mainDirection+2, 0, 3);
        var currentSideDirection = sideDirection;
        
        var deviatedDirection = Direction.UNKNOWN;
        
        var currentBranch = new Option<Branch>(root1);
        while (currentBranch.IsSome()) {
            if (currentBranch.Get().direction != mainDirection) {
                deviatedDirection = currentBranch.Get().direction;
                break;
            }else {
                currentBranch = currentBranch.Get().Next();
            }
        }
        var currentDirection = root1.direction;

        List<Neighbor> neighbors;
        for (int i  = 0; i < neighbors1.Count; i++) {
            neighbors = neighbors1[i].Tile.GetNeighbors();
            
            if (i+1 > neighbors1.Count-1) {
                EnableNeighbor(currentSideDirection, neighbors, neighbors2);
                continue;
            }

            if (neighbors1[i+1].Direction != currentDirection && neighbors1[i].Direction == currentDirection) {

                var nb = EnableNeighbor(currentSideDirection, neighbors, neighbors2);
                nb.GetAndThen(n => {
                    var nbs = n.Tile.GetNeighbors();

                    var nb2 = EnableNeighbor(currentDirection, nbs, neighbors2);
                    nb2.GetAndThen(n2 => {
                        EnableNeighbor(mainDirection, n2.Tile.GetNeighbors(), neighbors2);
                    });
                });

                currentSideDirection = currentSideDirection == sideDirection ? deviatedSideDirection : sideDirection;
                currentDirection = currentDirection == mainDirection ? deviatedDirection : mainDirection;
            }else {
                EnableNeighbor(currentSideDirection, neighbors, neighbors2);
            }
        }

        if (neighbors1[neighbors1.Count-1].Direction != mainDirection) {
            var nbs = neighbors2[neighbors2.Count-1].Tile.GetNeighbors();
            EnableNeighbor(sideDirection, nbs, neighbors2);
            nbs = neighbors1[neighbors1.Count-1].Tile.GetNeighbors();
            EnableNeighbor(sideDirection, nbs, neighbors2);
        }

        root2 = CreateBranches(neighbors2);
    }

    private Option<Neighbor> EnableNeighbor(Direction direction, List<Neighbor> buffer, List<Neighbor>neighbors) {
        foreach (var nb in buffer) {
            if (nb.Direction == direction) {
                nb.Tile.Enable();
                neighbors.Add(nb);
                return new Option<Neighbor>(nb);
            }
        }

        return Option<Neighbor>.None();
    }

    private Branch CreateBranches(List<Neighbor> neighbors) {
        var deviatedDirection = Direction.UNKNOWN;
        foreach (var nb in neighbors) {
            if (nb.Direction != mainDirection) {
                deviatedDirection = nb.Direction;
                break;
            }
        }
        var currentDirection = neighbors[0].Direction;

        Option<Branch> root = Option<Branch>.None(), current = Option<Branch>.None();

        for (int i = 1; i < neighbors.Count; i++) {
            if (i+1 >= neighbors.Count-1) {
                continue;
            }
            if (neighbors[i+1].Direction != currentDirection && neighbors[i].Direction == currentDirection) {
                if (root.IsNone()) {
                    var start = FindStart(neighbors);
                    Assert.IsNotNull(start, "start was null");
                    root = new Option<Branch>(new Branch(start.Get().transform.position, currentDirection, neighbors[i].Tile.transform.position));
                    current = root;
                }else {
                    current.GetAndThen(c => {
                        current = new Option<Branch>( new Branch(currentDirection, neighbors[i].Tile.transform.position, c) );
                    });
                }
            }

            currentDirection = currentDirection == mainDirection ? deviatedDirection : mainDirection;
        }

        AddBranch(neighbors, 1);

        var dir = wg.Tower.transform.position-neighbors1[neighbors1.Count-1].Tile.transform.position;
        if (dir.y > 0f) {
            dir.y = 0f;
        }
        var direction = Dir.FromVec(dir);
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA "+direction);
        if (currentBranch1.Get().direction != direction) {
            AddBranch(neighbors, direction, 1);
            currentBranch1.Get().end += dir.normalized*2f;
        }

        return root.Get();
    }

    public Interval StartSpawning(IRound round) {
        this.round = round;
        spawnInterval.Enable();
        spawnInterval.ResetEarly();
        return spawnInterval;
    }

    public void StopSpawning() {
        spawnInterval.Disable();
    }

    public void IntervalSpawnEnemy() {
        round.Spawn(root1, neighbors1[0].Tile.transform.position+Vector3.up*2.5f);
    }

    private void extend() {
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
            }
            // if (buffer.Count == 0) {
            //     break;
            // }

            var closestToTower = buffer[0];
            if (wg.IndexBelongsTo(neighbors1[neighbors1.Count-1].Tile.Index, 1)) {
                Option<Neighbor> a = Option<Neighbor>.None(), b = Option<Neighbor>.None();
                Direction perpendicular = Direction.UNKNOWN, perpendicular2 = Direction.UNKNOWN;
                if (neighbors1[1].Location.Type == LocationType.FIRST | neighbors1[1].Location.Type == LocationType.BOTTOM | neighbors1[1].Location.Type == LocationType.TOP) {
                    perpendicular = Direction.LEFT;
                    perpendicular2 = Direction.RIGHT;
                } else {
                    perpendicular = Direction.UP;
                    perpendicular2 = Direction.DOWN;
                }
                foreach (var nb in buffer) {
                    if (nb.Direction == perpendicular) {
                        a = new Option<Neighbor>(nb);
                    }else if (nb.Direction == perpendicular2) {
                        b = new Option<Neighbor>(nb);
                    }
                }

                if (a.IsNone() && b.IsSome()) {
                    closestToTower = b.Get();
                }else if (b.IsNone() && a.IsSome()) {
                    closestToTower = a.Get();
                }else if (b.IsSome() && a.IsSome()) {
                    closestToTower = a.Get().Tile.Distance > b.Get().Tile.Distance ? b.Get() : a.Get();
                }
            }

            if (currentDirection == Direction.UNKNOWN) {
                currentDirection = closestToTower.Direction;
            }

            if (currentDirection == closestToTower.Direction) {
                EnableNeighbor(closestToTower);
                continue;
            }
            EnableNeighbor(closestToTower);
            AddBranch(neighbors1);
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

        AddBranch(neighbors1, 1);
    }

    public Option<Tile> FindStart(List<Neighbor> neighbors, int offset = 0) {
        var direction = neighbors[neighbors.Count-2+offset].Direction;
        for (int i = neighbors.Count-2+offset; i >= 0; i--) {
            if (neighbors[i].Direction != direction) {
                return new Option<Tile>(neighbors[i+1].Tile);
            }
        }

        return Option<Tile>.None();
    }

    public void AddBranch(List<Neighbor> neighbors, int offset = 0) {
        if (currentBranch1.IsNone()) {
            var start = FindStart(neighbors1);
            Assert.IsNotNull(start.Get(), "Start was null");
            root1 = new Branch(start.Get().transform.position, neighbors1[neighbors1.Count-2].Direction, neighbors1[neighbors1.Count-2+offset].Tile.transform.position);
            currentBranch1 = new Option<Branch>(root1);
        }else {
            currentBranch1 = new Option<Branch>(new Branch(neighbors1[neighbors1.Count-2+offset].Direction, neighbors1[neighbors1.Count-2+offset].Tile.transform.position, currentBranch1.Get()));
        }
    }

    public void AddBranch(List<Neighbor> neighbors, Direction dir, int offset = 0) {
        if (currentBranch1.IsNone()) {
            var start = FindStart(neighbors1);
            Assert.IsNotNull(start.Get(), "Start was null");
            root1 = new Branch(start.Get().transform.position, dir, neighbors1[neighbors1.Count-2+offset].Tile.transform.position);
            currentBranch1 = new Option<Branch>(root1);
        }else {
            currentBranch1 = new Option<Branch>(new Branch(dir, neighbors1[neighbors1.Count-2+offset].Tile.transform.position, currentBranch1.Get()));
        }
    }
    
    private void SetAmount(int value) {
        var min = 3;

        var max = Mathf.FloorToInt(value/2f)-2;
        if (max > 6) {
            max = 6;
        }
        if (min > max) {
            min = max;
        }

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

}
