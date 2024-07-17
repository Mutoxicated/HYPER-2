using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using TMPro;

public enum Direction {
    RIGHT,
    LEFT,
    UP,
    DOWN,
    UNKNOWN
}

public class Neighbor : IComparable<Neighbor> {
    public Location Location;
    public Direction Direction;
    public Tile Tile;

    public Neighbor(Direction direction, Tile tile, Location location) {
        Direction = direction;
        Tile = tile;
        Location = location;
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
    private Option<GameObject> occupiedWith;
    private readonly Color litColor = new Color(0.9f,0.06f,0.06f);
    private const float heightOffset = 1f;

    private Vector3 initPos;

    private WorldGenerator worldGenerator;
    private int layer;
    private int index;
    private float distanceFromTower;

    public int Layer {
        get { return layer; }
    }

    public int Index {
        get { return index; }
    }

    public float Distance {
        get { return distanceFromTower; }
    }

    public bool IsLit() {
        return lit;
    }

    public void CStart(WorldGenerator wg, float dist, int layer, int index) {
        worldGenerator = wg;
        distanceFromTower = dist;
        this.layer = layer;
        this.index = index;
        textIndex.text = ""+index;
    }

    public List<Neighbor> GetNeighbors() {
        List<Neighbor> neighbors = new List<Neighbor>();
        var location = worldGenerator.GetLocationOfIndex(index, layer);
        var context = new Context(location, Direction.UP);
        //Debug.Log("Index: "+index);

        void AssignDelta(Direction direction) {
            context.Direction = direction;
            var delta = worldGenerator.GetLayerDelta(layer, context);
            if (delta.IsSome()) {
                var tileLocation = index+delta.Get();
                //Debug.Log("Delta got: "+delta.Get());
                if (tileLocation >= 0 && tileLocation <= worldGenerator.Tiles.Length-1 && !worldGenerator.Tiles[tileLocation-1].IsLit()) {
                    neighbors.Add(new Neighbor(direction, worldGenerator.Tiles[tileLocation-1], context.Location));
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

    public void Occupy(GameObject go) {
        occupied = true;
        occupiedWith = new Option<GameObject>(go);
        gameObject.layer = EditorState.GetLayer(EditorLayer.OCCUPIED);
    }

    public void Deoccupy() {
        occupied = false;
        occupiedWith = Option<GameObject>.None();
        gameObject.layer = EditorState.GetLayer(EditorLayer.INTERACTABLE_ONE);
    }

    public void Enable() {
        if (lit) { return; }
        transform.position = initPos+Vector3.up*heightOffset;
        lit = true;
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_TWO);
        mc.ChangeColor(litColor, true);
    }

    public void Disable() {
        if (!lit) { return; }
        transform.position = initPos;
        lit = false;
        Go.SetLayerRecursive(gameObject, EditorLayer.INTERACTABLE_ONE);
        mc.ChangeColor(true);
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
