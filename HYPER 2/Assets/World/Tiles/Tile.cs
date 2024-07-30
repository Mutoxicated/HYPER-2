using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using TMPro;

[Serializable]
public enum Direction {
    RIGHT,
    DOWN,
    LEFT,
    UP,
    UNKNOWN
}

public class Neighbor : IComparable<Neighbor> {
    public Direction Direction;
    public Tile Tile;

    public Location Location {
        get { return Tile.Location; }
    }

    public Neighbor(Direction direction, Tile tile) {
        Direction = direction;
        Tile = tile;
    }

    int IComparable<Neighbor>.CompareTo(Neighbor nb) {
        if (Tile.Distance == nb.Tile.Distance) {
            return 0;
        }

        return Tile.Distance > nb.Tile.Distance ? 1 : -1;
    }
}

public class Tile : MonoBehaviour, IComparable<Tile>
{
    [SerializeField] private MaterialChanger mc;
    [SerializeField] private TMP_Text textIndex;

    private bool lit = false;
    private bool occupied = false;
    public bool Occupied {
        get { return occupied; }
    }
    private Option<GameObject> occupiedWith;
    private readonly Color litColor = new Color(0.9f,0.06f,0.06f);
    private readonly Color goldColor = new Color(0.9f, 0.7f, 0f);
    private const float heightOffset = 1f;

    private Color currentColor;

    private Vector3 initPos;

    private WorldGenerator worldGenerator;
    private int layer;
    /// <summary>
    /// DO "index-1" WHEN ACCESSING WORLD GENERATOR TILES 
    /// </summary>
    private int index;
    private float distanceFromTower;
    private Location location;

    public int Layer {
        get { return layer; }
    }

    public int Index {
        get { return index; }
    }

    public float Distance {
        get { return distanceFromTower; }
    }
    public Location Location {
        get { return location; }
    }

    public bool IsLit() {
        return lit;
    }

    public void CStart(WorldGenerator wg, float dist, int layer, int index) {
        currentColor = mc.Color;
        worldGenerator = wg;
        distanceFromTower = dist;
        this.layer = layer;
        this.index = index;
        location = worldGenerator.GetLocationOfIndex(index, layer);
        textIndex.text = ""+index;
    }

    public List<Neighbor> GetNeighbors() {
        List<Neighbor> neighbors = new List<Neighbor>();
        var context = new Context(location, Direction.UP);
        //Debug.Log("Index: "+index);

        void AssignDelta(Direction direction) {
            context.Direction = direction;
            var delta = worldGenerator.GetLayerDelta(layer, context);
            if (delta.IsSome()) {
                var tileLocation = index+delta.Get();
                //Debug.Log("Delta got: "+delta.Get());
                if (tileLocation >= 0 && tileLocation <= worldGenerator.Tiles.Length-1 && !worldGenerator.Tiles[tileLocation-1].IsLit()) {
                    neighbors.Add(new Neighbor(direction, worldGenerator.Tiles[tileLocation-1]));
                }
            }else {
                //Debug.Log("Ain't got no delta.");
            }
        }

        AssignDelta(Direction.UP);

        AssignDelta(Direction.DOWN);

        AssignDelta(Direction.RIGHT);

        AssignDelta(Direction.LEFT);

        neighbors.Sort();

        return neighbors;
    }

    public Option<Neighbor> GetNeighbor(Direction dir) {
        var delta = worldGenerator.GetLayerDelta(layer, new Context(location, dir));
        if (delta.IsSome()) {
            var tileLocation = index+delta.Get();
            //Debug.Log("Delta got: "+delta.Get());
            if (tileLocation >= 0 && tileLocation <= worldGenerator.Tiles.Length-1 && !worldGenerator.Tiles[tileLocation-1].IsLit()) {
                return new Option<Neighbor>(
                    new Neighbor(dir, worldGenerator.Tiles[tileLocation-1])
                );
            }
        }
        return Option<Neighbor>.None();
    }

    public void Occupy(GameObject go) {
        occupied = true;
        occupiedWith = new Option<GameObject>(go);
        Go.SetLayerRecursive(gameObject, EditorLayer.OCCUPIED);
    }

    public void Deoccupy() {
        occupied = false;
        occupiedWith = Option<GameObject>.None();
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_ONE);
    }

    public void Enable() {
        if (lit) { return; }
        if (occupied) {
            Destroy(occupiedWith.Get());
        }
        transform.position = initPos+Vector3.up*heightOffset;
        lit = true;
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_TWO);
        mc.ChangeColor(litColor, true);
        currentColor = litColor;
    }
    public void Enable(Color color) {
        if (lit) { return; }
        if (occupied) {
            Destroy(occupiedWith.Get());
        }
        transform.position = initPos+Vector3.up*heightOffset;
        lit = true;
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_TWO);
        mc.ChangeColor(color, true);
        currentColor = color;
    }

    public void Disable() {
        if (!lit) { return; }
        transform.position = initPos;
        lit = false;
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_ONE);
        mc.ChangeColor(true);
        currentColor = mc.Color;
    }

    public void Goldify(GameObject occupiedBy) {
        if (lit) { return; }

        Occupy(occupiedBy);
        mc.ChangeColor(goldColor, true);
        currentColor = goldColor;
    }

    public void UpdateColor() {
        mc.ChangeColor(Color.Lerp(currentColor, Color.white, MusicVisualization.T), true);
    }

    private void Awake() {
        initPos = transform.position;
    }

    // private void Update() {
    //     if (lit) {
    //         transform.position = Vector3.Lerp(transform.position, initPos+Vector3.up*heightOffset, Time.deltaTime);
    //     }else {
    //         transform.position = Vector3.Lerp(transform.position, initPos, Time.deltaTime);
    //     }
    // }

    int IComparable<Tile>.CompareTo(Tile b)
    {
        if (distanceFromTower == b.distanceFromTower) {
            return 0;
        }

        return distanceFromTower > b.distanceFromTower ? 1 : -1;
    }
}
