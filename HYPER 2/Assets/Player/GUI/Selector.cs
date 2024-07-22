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

    public void Delete() {
        if (selected.IsNone()) {
            return;
        }
        var towerPos = selected.Get().transform.position;
        Destroy(selected.Get());
        selected = Option<GameObject>.None();

        RaycastHit hit;
        Physics.Raycast(towerPos+Vector3.up, Vector3.down, out hit, Mathf.Infinity);
        hit.collider.transform.parent.GetComponent<Tile>().Deoccupy();
    }

    private GameObject storedGo;

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
            return;
        }
        storedGo = hits[0].collider.gameObject;

        if (storedGo.layer == EditorState.GetLayer(EditorLayer.OCCUPIED) | storedGo.layer == EditorState.GetLayer(EditorLayer.MINI_TOWER)) {
            selection.Disable();
        }else {
            UpdateSelection();
        }

        if (player.OnLeftClick) {
            qi.GetSelectedArsenal().GetAndThen(a => {
                if (storedGo.gameObject && storedGo.layer == EditorState.GetLayer(EditorLayer.MINI_TOWER)) {
                    return;
                }
                var instance = manager.World.InstantiateMiniTower(storedGo.transform.position, a, mt => { mt.CStart(manager); });
                hits[0].collider.transform.parent.GetComponent<Tile>().Occupy(instance.gameObject);
                selected = new Option<GameObject>(instance.gameObject);
                selectedArsenalInfo.Enable(instance);
            }).Else(() => {
                selectedArsenalInfo.Disable();
                selected = Option<GameObject>.None();
            }, storedGo.layer != EditorState.GetLayer(EditorLayer.MINI_TOWER));
        }
    }

    /// <summary>
    /// Only call when the hits[0] layer is:
    /// 1. Unoccupied
    /// 2. Not a mini tower
    /// </summary>
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
