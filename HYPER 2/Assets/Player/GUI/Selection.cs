using UnityEngine;

public class Selection : MonoBehaviour {
    [SerializeField] private Transform range;

    public void Enable(WorldGenerator WG, Vector3 position, float range) {
        this.range.localScale = new Vector3(WG.RangeSize(range), this.range.localScale.y, WG.RangeSize(range));
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