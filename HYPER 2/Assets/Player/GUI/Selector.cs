using Helpers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] private InputSystemUIInputModule input;
    [SerializeField] private QuickInventory qi;
    [SerializeField] private LayerMask tileSelectionMask;
    private Player player;
    private Ray ray;
    private RaycastHit[] hits = new RaycastHit[1];
    private int stored;

    private Manager manager;
    private Option<GameObject> selected = Option<GameObject>.None();
    
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

        if (selected.IsSome() && player.OnDelete) {
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
            Debug.Log("Instantiated mini tower", go);
            var arsenal = qi.GetSelectedArsenal();
            arsenal.GetAndThen(a => {
                var template = Resources.GetStatTemplate(a);
                if (Save.rData.Money >= template.worth) {
                    Player.SetMoneyRelative(-template.worth);
                }else {
                    return;
                }
                go.layer = EditorState.GetLayer(EditorLayer.OCCUPIED);
                player.InstantiateMiniTower(hits[0].transform.position+Vector3.up*0.5f, a, m => m.CStart(manager));
            });
        }

        hits = new RaycastHit[1];
    }
}
