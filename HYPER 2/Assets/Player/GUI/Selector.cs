using Helpers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] private InputSystemUIInputModule input;
    [SerializeField] private QuickInventory qi;
    [SerializeField] private LayerMask tileSelectionMask;
    [SerializeField] private Selection selection; 
    [SerializeField] private SelectedArsenalInfo selectedArsenalInfo;
    private Player player;
    private Ray ray;
    private RaycastHit[] hits = new RaycastHit[1];
    private int stored;

    private Manager manager;
    private Option<GameObject> selected = Option<GameObject>.None();
    private WorldGenerator wg;
    private Option<StatsTemplate> currentStat = Option<StatsTemplate>.None();
    
    public void DStart(Player player, Manager manager) {
        this.manager = manager;
        this.player = player;
        wg = manager.World.WG;
        selectedArsenalInfo.DStart(manager.GP);
        gameObject.SetActive(true);
    }

    public void Delete(GameObject go) {
        Debug.Log("Delete", go);
        var towerPos = go.transform.position;
        Destroy(go);
        selected = Option<GameObject>.None();

        var direction = (towerPos-player.Cam.transform.position).normalized;
        direction.x = 0f;
        direction.z = 0f;

        stored = Physics.RaycastNonAlloc(new Ray(towerPos, direction),hits,Mathf.Infinity, tileSelectionMask);
        go.GetComponent<Tile>().Deoccupy();
    }

    public void Update() {
        if (player.OnMiddleButton) {
            return;
        }
        ray = player.Cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        //Debug.Log(ray.direction);
        stored = Physics.RaycastNonAlloc(ray,hits,Mathf.Infinity, tileSelectionMask);
        if (stored == 0 | input.IsPointerOverGameObject(Mouse.current.deviceId)) {
            currentStat = Option<StatsTemplate>.None();
            selection.Disable();
            selectedArsenalInfo.Disable(false);
            if (!selectedArsenalInfo.Locked)
                selected = Option<GameObject>.None();
            return;
        }
    }

    private void UpdateSelection() {
        var arsenal = qi.GetSelectedArsenal();
        bool got = arsenal.GetAndThen(a => {
            currentStat = new Option<StatsTemplate>(Resources.GetStatTemplate(a));
            selection.Enable(wg, hits[0].transform.position+Vector3.up*0.5f, currentStat.Get().statsBase.values[3]*currentStat.Get().modifiers[3]);
        }).Successful();
        if (!got) {
            currentStat = Option<StatsTemplate>.None();
            selection.Disable();
        }
    }

    public void OnDeselect() {
        selected = Option<GameObject>.None();
    }
}
