using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    [SerializeField] private LayerMask tileSelectionMask;
    private Player player;
    private Ray ray;
    private RaycastHit[] hits = new RaycastHit[1];
    private int stored;

    private Manager manager;
    
    public void DStart(Player player, Manager manager) {
        this.manager = manager;
        this.player = player;
        gameObject.SetActive(true);
    }

    public void Update() {
        if (player.OnMiddleButton) {
            return;
        }
        ray = player.Cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        //Debug.Log(ray.direction);
        stored = Physics.RaycastNonAlloc(ray,hits,Mathf.Infinity, tileSelectionMask);
        if (stored == 0) {
            return;
        }
        //Debug.Log("stored");

        if (player.OnLeftClick) {
            player.InstantiateMiniTower(hits[0].transform.position+Vector3.up*0.5f, 0, m => m.CStart(manager));
        }

        hits = new RaycastHit[1];
    }
}
