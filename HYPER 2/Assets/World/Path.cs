using System.Collections;
using System.Collections.Generic;
using Helpers;
using Unity.VisualScripting;
using UnityEngine;
using static Helpers.General;

public class Branch {
    public Direction direction;
    public Tile end;
    public List<Branch> branches = new List<Branch>();

    public Branch(Direction direction, Tile tile) {
        this.direction = direction;
        end = tile;
    }

    public Branch(Direction direction, Tile tile, Branch parent) {
        this.direction = direction;
        end = tile;
        parent.SetBranch(this);
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
    private List<Neighbor> neighbors = new List<Neighbor>();
    private Direction mainDirection = Direction.UNKNOWN;
    private Direction currentDirection = Direction.UNKNOWN;
    private Branch root;
    public Branch Root {
        get { return root; }
    }
    private Option<Branch> currentBranch = Option<Branch>.None();

    private int amount = 0;

    private List<Neighbor> buffer = new List<Neighbor>();

    private int noFinds = 0;

    private Manager manager;
    private IRound round;

    public void DestroyPath() {
        foreach (var nb in neighbors) {
            nb.Tile.Disable();
        }
        Destroy(gameObject);
    }

    public void CStart(Manager manager, Tile start) {
        this.manager = manager;
        world = manager.World;
        wg = world.WG;
        start.Enable();
        var location = wg.GetLocationOfIndex(start.Index, start.Layer);
        neighbors.Add(new Neighbor(Direction.UNKNOWN, start, location));

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

        var dir = wg.Tower.transform.position-neighbors[neighbors.Count-1].Tile.transform.position;
        var direction = Dir.FromVec(dir);
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA "+direction);
        if (currentBranch.Get().direction != direction) {
            AddBranch(direction, 1);
        }
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
        round.Spawn(root, neighbors[0].Tile.transform.position+Vector3.up*2.5f);
    }

    private void extend() {
        int noFinds = 0;
        while (validForExtending(neighbors[neighbors.Count-1].Tile)) {
            if (buffer.Count == 0)
                buffer = neighbors[neighbors.Count-1].Tile.GetNeighbors();
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
                    Debug.Log("Didn't find at: "+neighbors[neighbors.Count-1].Tile.Index+" with direction "+currentDirection);
                    if (noFinds > 2) {
                        break;
                    }
                    noFinds++;
                    amount = 0;
                    DisableNeighbor(neighbors[neighbors.Count-1]);
                    continue;
                }
            }
            // if (buffer.Count == 0) {
            //     break;
            // }

            var closestToTower = buffer[0];
            if (wg.IndexBelongsTo(neighbors[neighbors.Count-1].Tile.Index, 1)) {
                Option<Neighbor> a = Option<Neighbor>.None(), b = Option<Neighbor>.None();
                Direction perpendicular = Direction.UNKNOWN, perpendicular2 = Direction.UNKNOWN;
                if (neighbors[1].Location.Type == LocationType.FIRST | neighbors[1].Location.Type == LocationType.BOTTOM | neighbors[1].Location.Type == LocationType.TOP) {
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
            AddBranch();
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

        AddBranch(1);
    }

    public void AddBranch(int offset = 0) {
        if (currentBranch.IsNone()) {
            root = new Branch(neighbors[neighbors.Count-2].Direction, neighbors[neighbors.Count-2+offset].Tile);
            currentBranch = new Option<Branch>(root);
        }else {
            currentBranch = new Option<Branch>(new Branch(neighbors[neighbors.Count-2+offset].Direction, neighbors[neighbors.Count-2+offset].Tile, currentBranch.Get()));
        }
    }

    public void AddBranch(Direction dir, int offset = 0) {
        if (currentBranch.IsNone()) {
            root = new Branch(dir, neighbors[neighbors.Count-2+offset].Tile);
            currentBranch = new Option<Branch>(root);
        }else {
            currentBranch = new Option<Branch>(new Branch(dir, neighbors[neighbors.Count-2+offset].Tile, currentBranch.Get()));
        }
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

        amount = Seed.IntRange(min, max);
    }

    private void debugExtend() {
        buffer = neighbors[neighbors.Count-1].Tile.GetNeighbors();
        foreach (var nb in buffer) {
            nb.Tile.Enable();
            neighbors.Add(nb);
        }
    }

    private Message EnableNeighbor(Neighbor nb) {
        //Debug.Log("Tile index: "+nb.Tile.Index);
        //Debug.Log("Though to be "+nb.Direction+" to: "+neighbors[neighbors.Count-1].Tile.Index);
 
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
        neighbors.Add(nb);
        buffer.Clear();
        return Message.ALL_GOOD;
    }

    private void DisableNeighbor(Neighbor nb) {
        nb.Tile.Disable();
        neighbors.Remove(nb);
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
