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
        selectedArsenalInfo.DStart();
        gameObject.SetActive(true);
    }

    public void Delete() {
        Debug.Log("Delete", hits[0].collider.gameObject);
        var towerPos = selected.Get().transform.parent.position;
        Destroy(selected.Get().transform.parent.gameObject);
        selected = Option<GameObject>.None();

        var direction = (towerPos-player.Cam.transform.position).normalized;
        direction.x = 0f;
        direction.z = 0f;

        stored = Physics.RaycastNonAlloc(new Ray(towerPos, direction),hits,Mathf.Infinity, tileSelectionMask);
        hits[0].collider.gameObject.layer = EditorState.GetLayer(EditorLayer.INTERACTABLE_ONE);
    }

    public void Update() {
        if (player.OnMiddleButton) {
            return;
        }
        ray = player.Cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        //Debug.Log(ray.direction);
        stored = Physics.RaycastNonAlloc(ray,hits,Mathf.Infinity, tileSelectionMask);
        if (stored == 0) {
            currentStat = Option<StatsTemplate>.None();
            selection.Disable();
            return;
        }
        //Debug.Log("stored");

        if (selected.IsSome()) {
            if (player.OnDelete) {
                Delete();
            }else {
                if (!selectedArsenalInfo.gameObject.activeInHierarchy) {
                    selectedArsenalInfo.Enable(selected.Get().GetComponent<MiniTower>());
                }
            }
        }else {
            if (selectedArsenalInfo.gameObject.activeInHierarchy) {
                selectedArsenalInfo.Disable();
            }
            var arsenal = qi.GetSelectedArsenal();
            bool got = arsenal.GetAndThen(a => {
                currentStat = new Option<StatsTemplate>(Resources.GetStatTemplate(a));
                selection.Enable(wg, hits[0].transform.position+Vector3.up*0.5f, currentStat.Get().values[3]);
            });
            if (!got) {
                currentStat = Option<StatsTemplate>.None();
                selection.Disable();
            }
        }

        if (player.OnLeftClick) {
            Debug.Log("Clicked");
            var go = hits[0].collider.gameObject;
            if (go.layer == EditorState.GetLayer(EditorLayer.OCCUPIED)) {
                Debug.Log("Object is occupied ("+go.name+")", go);
                selected = Option<GameObject>.None();
                return;
            }else if (go.layer == EditorState.GetLayer(EditorLayer.MINI_TOWER)) {
                Debug.Log("Object is mini tower ("+go.name+")", go);
                selected = new Option<GameObject>(go);
                return;
            }
            if (input.IsPointerOverGameObject(Mouse.current.deviceId)) {
                return;
            }
            var arsenal = qi.GetSelectedArsenal();
            arsenal.GetAndThen(a => {
                var template = Resources.GetStatTemplate(a);
                if (Save.rData.Money >= template.worth) {
                    Player.SetMoneyRelative(-template.worth);
                }else {
                    return;
                }
                Debug.Log("Instantiated mini tower", go);
                go.layer = EditorState.GetLayer(EditorLayer.OCCUPIED);
                var mt = player.InstantiateMiniTower(hits[0].transform.position+Vector3.up*0.5f, a, m => m.CStart(manager));
                selected = new Option<GameObject>(mt.gameObject);
            });
        }

        hits = new RaycastHit[1];
    }

    public void OnDeselect() {
        selected = Option<GameObject>.None();
    }
}
