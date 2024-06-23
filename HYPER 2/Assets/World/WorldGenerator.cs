using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int layers = 4;

    private int diagonalTileCount = 10;
    private float towerSize;
    private float tileSize;

    private static readonly int initSideTileCount = 3;
    private readonly int sides = 4;

    private int currentSideTiles = 1;
    private int currentLayer = 0;

    private Vector3 previousPos = Vector3.zero;
    private GameObject currentTile;

    private void Start() {
        towerSize = tower.transform.localScale.x*0.5f;
        tileSize = tilePrefab.transform.GetChild(0).transform.localScale.x*0.5f;

        Layer();
    }

    // private void AddTile() {
    //     currentTile = Instantiate(tilePrefab, previousPos, Quaternion.identity);
    // }
    private void AddTile(Vector3 direction) {
        currentTile = Instantiate(tilePrefab, previousPos, Quaternion.identity);
        currentTile.transform.position += direction*tileSize*2;
        previousPos = currentTile.transform.position;
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
}
