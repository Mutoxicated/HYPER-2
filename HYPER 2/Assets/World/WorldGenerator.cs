using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int layers = 1;

    private List<Tile> tiles = new List<Tile>();
    public Tile[] Tiles {
        get { return tiles.ToArray(); }
    }

    public GameObject Tower {
        get { return tower; }
    }

    private float towerSize;
    private float tileSize;

    private int currentSideTiles = 1;
    private int currentLayer = 0;

    private Vector3 previousPos = Vector3.zero;
    private GameObject currentTile;

    public void DStart() {
        towerSize = tower.transform.localScale.x*0.5f;
        tileSize = tilePrefab.transform.GetChild(0).transform.localScale.x*0.5f;

        Layer();

        CreatePaths();
    }

    public float GetLayerRadius() {
        return (tileSize*2+layers*tileSize*2)*Mathf.Sqrt(2);
    }

    private void AddTile(Vector3 direction) {
        currentTile = Instantiate(tilePrefab, previousPos, Quaternion.identity);
        currentTile.transform.position += direction*tileSize*2;
        previousPos = currentTile.transform.position;
        var tile = currentTile.GetComponent<Tile>();
        tiles.Add(tile);
        tile.SetIndex(tiles.Count-1);
        tile.SetLayer(currentLayer);
        tile.SetWorldGenerator(this);
    }

    private void Layer() {
        previousPos = new Vector3(towerSize+tileSize*currentSideTiles,0,towerSize+tileSize*currentSideTiles);
        currentSideTiles += 2;
        currentLayer++;

        for (int i = 0; i < currentSideTiles; i++) {
            AddTile(Vector3.left);
        }
        for (int i = 0; i < currentSideTiles; i++) {
            AddTile(Vector3.back);
        }
        for (int i = 0; i < currentSideTiles; i++) {
            AddTile(Vector3.right);
        }
        for (int i = 0; i < currentSideTiles; i++) {
            AddTile(Vector3.forward);
        }

        if (currentLayer < layers) {
            Layer();
        }
    }

    private int LayerAmount(int layer) { // CORRECT
        return 4+(layer*8);
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
        53 52 51 50 49 48 47 46
        54 27 26 25 24 23 22 45
        55 28 9  8   7  6 21 44
        56 29 10|     |[5(20]43)
        57 30 11|_____| 4 19 42
        58 31 12  1  2  3 18 41   -| 13
        59 32 13 14 15 16 17 40   -| 21
        60 33 34 35 36 37 38 39   -|

        Corners:
            Layer 1: [1] 3, 6, 9, 12 (+3) ----|
            Layer 2: [13] 17, 22, 27, 32 (+5)  |--- (+2/layer) || (1)3-1 = 2, (2)17-13 = 4, (3)39-33 = 6
            Layer 3: [33] 39, 46, 53, 60 (+7)--|                  |_____________ +2/layer _____________|

        Directions:
            DOWN: 
                <= 1st Corner: Increase 
                > 1st <= 2nd: Decrease by 1
                > 2nd < 3rd: Decrease
                >= 3rd <= 4th: Increase by 1
            UP: 
                The opposite of DOWN
            RIGHT:
                <= 1st Corner: Increase by 1
                >= 1st <= 2nd: Increase
                > 2nd <= 3rd: Decrease by 1
                > 3rd <= 4th: Decrease
            LEFT:
                The opposite of RIGHT

    */

//                                             0  12  20  28  36
//                                               /   /   /   /
    private int GetFirstOfLayer(int layer) {// 0, 12, 32, 60, 96, 
        return GetLastOfLayer(layer-1)+1;   // 0, 1,  2,  3,  4
    }

    private List<int> LayerRange(int layer) {
        var first = GetFirstOfLayer(layer);
        var last = GetLastOfLayer(layer);
        var range = new List<int>();
        for (int i = 0; i <= last-first; i++) {
            range.Add(i+first-1);
        }
        return range;
    }

    private void EnableFourTiles(List<int> range, int tilesEnabled, ref List<int> enabledTileIndexes) {
        var modifiedRange = enabledTileIndexes.Count % 2 == 0 ? range.ToArray().Reverse() : range.ToArray();
        foreach (var index in range.ToArray()) {
            if (Seed.Chance(4f/range.Count)) {
                tiles[index].Enable();
                enabledTileIndexes.Add(index);
                range.Remove(index);
                tilesEnabled++;
            }
            if (tilesEnabled > 3) {
                break;
            }
        }
        if (tilesEnabled < 4) {
            EnableFourTiles(range, tilesEnabled, ref enabledTileIndexes);
        }
    }

    private void CreatePaths() {
        // Enable 4 random tiles from the last layer
        var range = LayerRange(2);
        Debug.Log("Tiles: "+tiles.Count);
        Debug.Log("Range: {"+range[0]+", "+range[range.Count-1]+"}");
        var tileIndexes = new List<int>();
        EnableFourTiles(range, 0, ref tileIndexes);

    }
}
