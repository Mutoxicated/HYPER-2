using Unity.VisualScripting;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private WorldGenerator worldGenerator;
    private float borderRadius;

    public float BorderRadius {
        get { return borderRadius; }
    }

    public GameObject Tower {
        get { return worldGenerator.Tower; }
    }

    public void DStart(Player player) {
        worldGenerator.DStart(player);
        borderRadius = worldGenerator.GetLayerRadius();
        //Debug.Log(borderRadius);
    }
}
