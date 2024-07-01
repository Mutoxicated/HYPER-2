using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helpers.General;

public class Path : MonoBehaviour
{
    private WorldGenerator wg;
    private List<Neighbor> neighbors = new List<Neighbor>();
    private Direction mainDirection = Direction.UNKNOWN;

    private Direction directionToTake = Direction.UNKNOWN;
    private int amount = 0;

    public void DestroyPath() {
        foreach (var nb in neighbors) {
            nb.Tile.Disable();
        }
        Destroy(gameObject);
    }

    public void CStart(WorldGenerator wg, Tile start) {
        this.wg = wg;
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
            this.mainDirection = Direction.DOWN;
            break;
        case LocationType.TOP_LEFT:
            mainDirection = FiftyFifty(Direction.RIGHT, Direction.DOWN);
            break;
        case LocationType.LEFT:
            this.mainDirection = Direction.RIGHT;
            break;
        }

        extend();
    }

    private void extend() {
        while (validForExtending(neighbors[neighbors.Count-1].Tile)) {
            var localNeighbors = neighbors[neighbors.Count-1].Tile.GetNeighbors();
            if (amount > 0) {
                Debug.Log("Um?");
                var leave = false;
                foreach (var nb in localNeighbors) {
                    if (nb.Direction == directionToTake) {
                        EnableNeighbor(nb);
                        Debug.Log("---/Um?");
                        amount--;
                        leave = true;
                        break;
                    }
                }
                if (leave) {
                    continue;
                }
            }
            if (localNeighbors.Count == 0) {
                break;
            }

            var closestToTower = localNeighbors[0];

            if (mainDirection != closestToTower.Direction) {
                EnableNeighbor(closestToTower);

                var range = closestToTower.Location.CornersRange;
                range.a = Mathf.Abs(range.a);
                range.b = Mathf.Abs(range.b);
                
                directionToTake = closestToTower.Direction;
                amount = range.a > range.b ? Random.Range(Mathf.Clamp(3, 3, range.a), Mathf.Clamp(5, 5, range.a)) :  Random.Range(Mathf.Clamp(3, 3, range.b), Mathf.Clamp(5, 5, range.b));
                Debug.Log("Amount: "+amount);
            }else {
                EnableNeighbor(closestToTower);
            }
        }
    }

    private void debugExtend() {
        var localNeighbors = neighbors[neighbors.Count-1].Tile.GetNeighbors();
        foreach (var nb in localNeighbors) {
            nb.Tile.Enable();
            neighbors.Add(nb);
        }
    }

    private void EnableNeighbor(Neighbor nb) {
        Debug.Log("Tile index: "+nb.Tile.Index);
        Debug.Log("Though to be "+nb.Direction+" to: "+neighbors[neighbors.Count-1].Tile.Index);
        nb.Tile.Enable();
        neighbors.Add(nb);
    }

    private bool validForExtending(Tile tile) {
        if (tile.Layer == 1) {
            int location = (int)wg.GetLocationOfIndex(tile.Index, tile.Layer).Type;
            if (location == 0) {
                return true;
            }
            return location % 2 == 0;
        }
        return true;
    }

}
