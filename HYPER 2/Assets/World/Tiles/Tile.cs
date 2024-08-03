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

[Serializable]
public class Neighbor : IComparable<Neighbor> {
    public Direction Direction;
    public int tileIndex;
    public float dist;

    public Tile this[WorldGenerator wg] {
        get { return wg.Tiles[tileIndex-1]; }
    }

    public Neighbor(Direction direction, Tile tile) {
        Direction = direction;
        tileIndex = tile.Index;
        dist = tile.Distance;
    }

    int IComparable<Neighbor>.CompareTo(Neighbor nb) {
        if (dist == nb.dist) {
            return 0;
        }

        return dist > nb.dist ? 1 : -1;
    }
}

public class Tile : MonoBehaviour, IComparable<Tile>
{
    [SerializeField] private MaterialChanger mc;
    [SerializeField] private TMP_Text textIndex;
    
    [SerializeField] private string dir;

    private bool lit = false;
    private bool occupied = false;
    public bool Occupied {
        get { return occupied; }
    }
    private Option<GameObject> occupiedWith;
    private Color normalColor;
    private readonly Color litColor = new Color(0.9f,0.06f,0.06f);
    private readonly Color goldColor = new Color(0.9f, 0.7f, 0f);
    private const float heightOffset = 1f;

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
        normalColor = mc.Color;
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

    public void Enable(string dir) {
        if (lit) { return; }
        if (occupied) {
            Destroy(occupiedWith.Get());
        }
        textIndex.text += String.Format("\n{0}", dir);
        transform.position = initPos+Vector3.up*heightOffset;
        lit = true;
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_TWO);
        mc.SetColor(litColor);
        mc.ChangeColorOverride();
    }
    public void Enable(Color color, string dir) {
        if (lit) { return; }
        textIndex.text += String.Format("\n{0}", dir);
        if (occupied) {
            Destroy(occupiedWith.Get());
        }
        transform.position = initPos+Vector3.up*heightOffset;
        lit = true;
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_TWO);
        mc.SetColor(color);
        mc.ChangeColorOverride();
    }

    public void Disable() {
        if (!lit) { return; }
        transform.position = initPos;
        lit = false;
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_ONE);
        mc.SetColor(normalColor);
        mc.ChangeColorOverride();
    }

    public void Goldify(GameObject occupiedBy) {
        if (lit) { return; }

        Occupy(occupiedBy);
        mc.SetColor(goldColor);
        mc.ChangeColorOverride();
    }

    public void UpdateColor() {
        mc.ChangeColorOverride(Color.Lerp(mc.Color, Color.white, MusicVisualization.T));
    }

    public void Write(string text) {
        textIndex.text += "\n"+text;
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
