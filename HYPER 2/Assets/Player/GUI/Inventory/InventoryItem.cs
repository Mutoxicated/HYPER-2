using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    private Resources.Arsenal arsenalType;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text entityName;
    private bool updatePos = false;
    private bool hovering = false;
    private Vector3 mousePos;
    private Option<Vector3> initPos = Option<Vector3>.None();
    private RectTransform rt;

    private Inventory inv;

    public Resources.Arsenal ArsenalType {
        get { return arsenalType; }
    }

    public void CStart(Inventory inv, Resources.Arsenal arsenalType) {
        this.inv = inv;
        rt = GetComponent<RectTransform>();
        this.arsenalType = arsenalType;
        image.sprite = Resources.GetArsenalSprite(arsenalType);
        entityName.text = Arsnl.ToName(arsenalType);
    }

    
    private void Update() {
        if (hovering) {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) {
                inv.QuickInventory.UpdateQI(inv.QuickInventory.Capacity-1, arsenalType);
            }else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
                inv.QuickInventory.UpdateQI(inv.QuickInventory.Capacity-2, arsenalType);
            }else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
                inv.QuickInventory.UpdateQI(inv.QuickInventory.Capacity-3, arsenalType);
            }else if (Keyboard.current.digit4Key.wasPressedThisFrame) {
                inv.QuickInventory.UpdateQI(inv.QuickInventory.Capacity-4, arsenalType);
            }else if (Keyboard.current.digit5Key.wasPressedThisFrame) {
                inv.QuickInventory.UpdateQI(inv.QuickInventory.Capacity-5, arsenalType);
            }
        }
        if (!updatePos) return;
        mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 1f;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnDisable() {
        if (updatePos)
            OnPointerUp();
        if (hovering)
            OnPointerExit();
    }

    public void OnPointerDown(PointerEventData _ = null)
    {
        //Debug.Log("drag");
        inv.currentlyDragged = new Option<InventoryItem>(this);
        if (initPos.IsNone()) {
            Debug.Log("position: "+rt.position);
            initPos = new Option<Vector3>(rt.position);
        }
        updatePos = true;
    }

    public void OnPointerUp(PointerEventData  _ = null)
    {
        inv.QuickInventory.CheckCDs();
        inv.currentlyDragged =  Option<InventoryItem>.None();
        updatePos = false;
        rt.position = initPos.Get();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData _ = null)
    {
        hovering = false;
    }
}