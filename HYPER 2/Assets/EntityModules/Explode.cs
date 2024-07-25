
using UnityEngine;

public class Explode : MonoBehaviour {
    [SerializeField] private GameObject explosion;

    private void OnDisable() {
        if(!gameObject.scene.isLoaded) return;
        explosion.transform.parent = null;
        explosion.SetActive(true);
    }

}
