using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;

public enum LocationType {// 0+0, 1+1, 2+2, 3+3
    FIRST,
    BOTTOM,
    BOTTOM_RIGHT,
    RIGHT,
    RIGHT_TOP,
    TOP,
    TOP_LEFT,
    LEFT,
    LEFT_BOTTOM
}

public struct Location {
    private LocationType type;
    private int less;
    private int greater;

    public LocationType Type {
        get { return type; }
    }

    public (int a, int b) CornersRange {
        get { return (less, greater); }
    }

    public Location(LocationType Type, int less, int greater) {
        type = Type;
        this.less = less;
        this.greater = greater;
    }
}

public class Context {
    private Location location;
    private Direction direction;

    public Location Location {
        get { return location; }
    }

    public Direction Direction {
        set { direction = value; }
        get { return direction; }
    }

    public Context(Location location, Direction direction) {
        this.location = location;
        this.direction = direction;
    }
}

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private World world;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject pathPrefab;
    [SerializeField] private int layers = 1;

    private List<Tile> tiles = new List<Tile>();
    public Tile[] Tiles {
        get { return tiles.ToArray(); }
    }

    public GameObject Tower {
        get { return world.Tower; }
    }

    // Tile generation
    private float towerSize;
    private float tileSize;

    private int currentSideTiles = 1;
    private int currentLayer = 0;

    private Vector3 previousPos = Vector3.zero;
    private GameObject currentTile;

    //Other
    private Player player;

    // Path generation
    private List<Path> paths = new List<Path>();
    private int lastRandomCorner;

    public float TileSize {
        get { return tileSize; }
    }

    public int LastLayer {
        get { return layers; }
    }

    public void DStart(World world, Player player) {
        this.player = player;
        this.world = world;
        towerSize = world.TowerPlatform.transform.localScale.x*0.5f;
        tileSize = tilePrefab.transform.GetChild(0).transform.localScale.x*0.5f;

        Layer();

        CreatePaths();
    }

    public void Update() {
        if (player.OnDebugRefresh) {
            foreach (var path in paths) {
                path.DestroyPath();
            }
            paths.Clear();

            CreatePaths();
        }
    }

    public int GetLayers() {
        return layers;
    }

    public float GetLayerRadius() {
        return (tileSize*2+layers*tileSize*2)*Mathf.Sqrt(2);
    }

    private void AddTile(Vector3 direction) {
        currentTile = Instantiate(tilePrefab, previousPos+direction*tileSize*2, Quaternion.identity);
        previousPos = currentTile.transform.position;
        var tile = currentTile.GetComponent<Tile>();
        tiles.Add(tile);
        tile.CStart(
            this, 
            Vector3.Distance(tile.transform.position, Tower.transform.position), 
            currentLayer, 
            tiles.Count
        );
    }

    private void Layer() {
        previousPos = new Vector3(-towerSize-tileSize*currentSideTiles,0,-towerSize-tileSize*currentSideTiles);
        currentSideTiles += 2;
        currentLayer++;

        for (int i = 0; i < currentSideTiles; i++) {
            AddTile(Vector3.right);
        }
        for (int i = 0; i < currentSideTiles; i++) {
            AddTile(Vector3.forward);
        }
        for (int i = 0; i < currentSideTiles; i++) {
            AddTile(Vector3.left);
        }
        for (int i = 0; i < currentSideTiles; i++) {
            AddTile(Vector3.back);
        }

        if (currentLayer < layers) {
            Layer();
        }
    }

    public bool IndexBelongsTo(int index, int layer) {
        return index >= GetFirstOfLayer(layer) && index <= GetLastOfLayer(layer);
    }

    public Location GetLocationOfIndex(int index, int layer) {
        if (index == GetFirstOfLayer(layer)) {
            return new Location(LocationType.FIRST, -1, GetLayerSurplus(layer)-1);
        }

        //Debug.Log("// Finding location of: "+index);
        int nth = 4;
        int corner = GetLayerCorner(layer, nth);
        while (index <= corner) {
            //Debug.Log("Corner: "+corner+" ["+nth+"]");
            if (index == corner) {
                //Debug.Log("// Is corner "+ nth);
                return new Location((LocationType)((nth)*2), 0, GetLayerSurplus(layer));
            }
            nth--;
            if (nth <= 0) {
                break;
            }
            corner = GetLayerCorner(layer, nth);
        }

        //Debug.Log("Corner: "+corner+" ["+nth+"]");

        int corner1Delta, corner2Delta = 0;
        if (nth == 0) {
            corner1Delta = -1;
        }else {
            corner1Delta = GetLayerCorner(layer, nth)-index;
        }
        corner2Delta = GetLayerCorner(layer, nth+1)-index;

        if (corner1Delta < corner2Delta) {
            return new Location((LocationType)(nth+1+nth), corner1Delta, corner2Delta);
        }else {
            return new Location((LocationType)(nth+1+nth), corner2Delta, corner1Delta);
        }
    }

    public int LayerAmount(int layer) { // CORRECT
        return 4+(layer*8);
    }

    public Option<int> GetLayerDelta(int layer, Context context) {
        int delta = LayerAmount(layer)+1;
        int prevDelta = LayerAmount(layer-1)+1;

        //Debug.Log("Getting delta ("+delta+")");
        //Debug.Log("- Context:");
        //Debug.Log("-- Location = "+context.Location.Type+", Direction = "+context.Direction);

        switch (context.Direction) {
        case Direction.DOWN:
            
            switch (context.Location.Type) {
            case LocationType.FIRST:
            case LocationType.BOTTOM:
            case LocationType.BOTTOM_RIGHT:
                return new Option<int>(delta);
            case LocationType.RIGHT:
            case LocationType.RIGHT_TOP:
                return new Option<int>(-1);
            case LocationType.TOP:
                if (layer == 1) {
                    return Option<int>.None();
                }
                return new Option<int>(-prevDelta-4);
            case LocationType.TOP_LEFT:
            case LocationType.LEFT:
            case LocationType.LEFT_BOTTOM:
                return new Option<int>(1);
            default:
                return Option<int>.None();
            }
        case Direction.UP:
            switch (context.Location.Type) {
            case LocationType.LEFT:
            case LocationType.LEFT_BOTTOM:
                return new Option<int>(-1);
            case LocationType.FIRST:
                if (layer == 1) {
                    return Option<int>.None();
                }
                return new Option<int>(-1);
            case LocationType.BOTTOM:
                if (layer == 1) {
                    return Option<int>.None();
                }
                return new Option<int>(-prevDelta);
            case LocationType.BOTTOM_RIGHT:
            case LocationType.RIGHT:
                return new Option<int>(1);
            case LocationType.RIGHT_TOP:
            case LocationType.TOP:
            case LocationType.TOP_LEFT:
                return new Option<int>(delta+4);
            default:
                return Option<int>.None();
            }
        case Direction.RIGHT:
            switch (context.Location.Type) {
            case LocationType.FIRST:
            case LocationType.BOTTOM:
                return new Option<int>(1);
            case LocationType.BOTTOM_RIGHT:
            case LocationType.RIGHT:
            case LocationType.RIGHT_TOP:
                return new Option<int>(delta+2);
            case LocationType.TOP:
            case LocationType.TOP_LEFT:
                return new Option<int>(-1);
            case LocationType.LEFT:
                if (layer == 1) {
                    return Option<int>.None();
                }
                return new Option<int>(-prevDelta-6);
            case LocationType.LEFT_BOTTOM:
                return new Option<int>(-delta+2);
            default:
                return Option<int>.None();;
            }
        case Direction.LEFT:
            switch (context.Location.Type) {
            case LocationType.FIRST:
                return new Option<int>(delta-2);
            case LocationType.BOTTOM:
            case LocationType.BOTTOM_RIGHT:
                return new Option<int>(-1);
            case LocationType.RIGHT:
                if (layer == 1) {
                    return Option<int>.None();
                }
                return new Option<int>(-prevDelta-2);
            case LocationType.RIGHT_TOP:
            case LocationType.TOP:
                return new Option<int>(1);
            case LocationType.TOP_LEFT:
            case LocationType.LEFT:
            case LocationType.LEFT_BOTTOM:
                return new Option<int>(delta+6);
            default:
                return Option<int>.None();
            }
        default:
            return Option<int>.None();
        }
    }

    private int GetLayerSurplus(int layer) {
        return 3 + ( layer - 1 ) * 2;
    }

    private int GetLayerCorner(int layer, int nth) {
        return GetFirstOfLayer(layer) + 2*layer + ( nth-1 )*GetLayerSurplus(layer) ;
    }

    private int GetLastOfLayer(int layer) {
        if (layer == 0) {
            return 0;
        }
        return LayerTotal(layer);
    }

    private int LayerTotal(int layer) {
        return 4*layer + layer*(8+layer*8)/2;
    }

    /*       
        Amount:
            Layer 1: 12 (4+8*1)
            Layer 2: 20 (4+8*2)
            Layer 3: 28 (4+8*3)
        Range:
            Layer 1: 4+8 
            Layer 2: 4+8 + 4+16 = 32 
            Layer 3: 4+8 + 4+16 + 4+24 = n*4 + n*(8+n*8)/2

        
           -19           15 23
           |  |         |  |  |
        87 86 85 84 83 82 81 80 79 78
        88 53 52 51 50 49 48 47 46 77
        89 54 27 26 25 24 23 22 45 76
        90 55 28 9  8   7  6 21 44 75
        91 56 29 10|     |[5(20]43)74
        92 57 30 11|_____| 4 19 42 73
        93 58 31 12  1  2  3 18 41 72    -| 13 (Layer 1 Amount + 1)
        94 59 32 13 14 15 16 17 40 71   -| 21 (Layer 2 Amount + 1)
        95 60 33 34 35 36 37 38 39 70  -| 29 (Layer 3 Amount + 1)
        96 61 62 63 64 65 66 67 68 69 -|

        Corners:
            Layer 1: [1] 3, 6, 9, 12 (+3) -----|
            Layer 2: [13] 17, 22, 27, 32 (+5)  |--- (+2/layer) || (1)3-1 = 2, (2)17-13 = 4, (3)39-33 = 6
            Layer 3: [33] 39, 46, 53, 60 (+7)--|                  |_____________ +2/layer _____________|

        Directions (LAM = Layer AMount, PREV = Previous Layer)
            DOWN: 
                <= 1st Corner: Increase by LAM+1
                > 1st <= 2nd: Decrease by 1
                > 2nd < 3rd: Decrease by (PREV_LAM+1) + 2*PASSED_CORNERS
                >= 3rd <= 4th: Increase by 1
            UP: 
                1st Elem: Decrease by 1
                > 1st Elem < 1st Corner: Decrease by PREV_LAM+1
                >= 1st < 2nd: Increase by 1
                >= 2nd <= 3rd: Increase by (PREV_LAM+1) + 2*PASSED_CORNERS
                > 3rd <= 4th: Decrease by 1
            RIGHT:
                < 1st Corner: Increase by 1
                >= 1st <= 2nd: Increase by (LAM+1) + 2*PASSED_CORNERS
                > 2nd <= 3rd: Decrease by 1
                > 3rd <= 4th: Decrease by (PREV_LAM+1) + 2*PASSED_CORNERS
            LEFT:
                1st Elem: Increase by (LAM+1)-2
                > 1st Elem <= 1st Corner: Decrease by 1
                > 1st < 2nd: Decrease by (PREV_LAM+1) + 2*PASSED_CORNERS
                >= 2nd < 3rd: Increase by 1
                >= 3rd <= 4th: Increase by (LAM+1) + 2*PASSED_CORNERS

    */

//                                             0  12  20  28  36
//                                               /   /   /   /
    private int GetFirstOfLayer(int layer) {// 0, 12, 32, 60, 96, 
        return GetLastOfLayer(layer-1)+1;   // 0, 1,  2,  3,  4
    }

    public List<int> LayerRange(int layer) {
        var first = GetFirstOfLayer(layer);
        var last = GetLastOfLayer(layer);
        var range = new List<int>();
        for (int i = 0; i <= last-first; i++) {
            range.Add(i+first-1);
        }
        return range;
    }

    private void InstantiatePath(int index) {
        var instance = Instantiate(pathPrefab, Vector3.zero, Quaternion.identity);
        var path = instance.GetComponent<Path>();
        paths.Add(path);
        path.CStart(player.Manager, tiles[index]);
    }

    private void CreatePaths() {
        int randomCorner = Seed.IntRange(1,5);
        int randomCorner2 = Num.Wrap(randomCorner+1, 1, 4);
        int randomCorner3 = Num.Wrap(randomCorner2+1, 1, 4);
        //int randomCorner4 = Num.Wrap(randomCorner3+1, 1, 4);
        lastRandomCorner = randomCorner3;
        //Debug.Log("first: "+randomCorner+", second: "+randomCorner2+", third: "+randomCorner3+", fourth: "+randomCorner4);

        InstantiatePath(GetRandomIndexFromCorners(layers, randomCorner, randomCorner2));
        InstantiatePath(GetRandomIndexFromCorners(layers, randomCorner2, randomCorner3));
        //InstantiatePath(GetRandomIndexFromCorners(layers, randomCorner3, randomCorner4));
    }

    public void CreateNextPath() {
        int next = Num.Wrap(lastRandomCorner+1, 1, 4);

        InstantiatePath(GetRandomIndexFromCorners(layers, lastRandomCorner, next));
        lastRandomCorner = next;
    }

    private int GetRandomIndexFromCorners(int layer, int a, int b) {
        int min, max;
        if ((a == 1 && b == 4) | (b == 1 && a == 4)) {
            min = GetFirstOfLayer(layer);
            max = GetLayerCorner(layer, 1)-1;
        }else {
            min = GetLayerCorner(layer, a)+1;
            max = GetLayerCorner(layer, b)-4;
        }

        return Seed.IntRange(min, max);
    }
}
