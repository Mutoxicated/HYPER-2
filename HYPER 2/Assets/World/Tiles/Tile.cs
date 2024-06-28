using Helpers;
using UnityEngine;

public enum Direction {
    RIGHT,
    LEFT,
    TOP,
    BOTTOM
}

public class Neighbor {
    public Direction direction;
    public Tile tile;

    public Neighbor(Direction direction, Tile tile) {
        this.direction = direction;
        this.tile = tile;
    }
}

public class Tile : MonoBehaviour
{
    [SerializeField] private MaterialChanger mc;

    private readonly Color enabledColor = Color.red;
    private const float heightOffset = 10f;

    private Vector3 initPos;

    private WorldGenerator worldGenerator;
    private int layer;
    private int index;

    public void SetWorldGenerator(WorldGenerator wg) {
        worldGenerator = wg;
    }

    public void SetLayer(int layer) {
        this.layer = layer;
    }

    public void SetIndex(int index) {
        this.index = index;
    }

    public void GetNeighbors() {

    }

    public void Enable() {
        Debug.Log("happened");
        enabled = true;
        gameObject.layer = EditorState.GetLayer(EditorLayer.INTERACTABLE_ONE);
        mc.ChangeColor(enabledColor, true);
    }

    private void Start() {
        enabled = false;
        initPos = transform.position;
    }

    private void Update() {
        if (transform.position.y > initPos.y+heightOffset) {
            enabled = false;
            return;
        }
        transform.position = Vector3.Lerp(transform.position, initPos+Vector3.up*heightOffset, Time.deltaTime);
    }
}
