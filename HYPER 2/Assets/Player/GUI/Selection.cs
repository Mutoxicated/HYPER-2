using UnityEngine;

public class Selection : MonoBehaviour {
    [SerializeField] private Transform range;

    public void Enable(WorldGenerator WG, Vector3 position, float range) {
        this.range.localScale = new Vector3(range*WG.TileSize*4-WG.TileSize, this.range.localScale.y, range*WG.TileSize*4-WG.TileSize);
        transform.position = position;
        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }
    }

    public void Disable() {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }
}