using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    private Resources.Arsenal arsenalType;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text entityName;
    private bool updatePos;
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
        if (!updatePos) return;
        mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 1f;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("drag");
        inv.currentlyDragged = new Option<InventoryItem>(this);
        if (initPos.IsNone()) {
            Debug.Log("position: "+rt.position);
            initPos = new Option<Vector3>(rt.position);
        }
        updatePos = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inv.QuickInventory.CheckCDs();
        inv.currentlyDragged =  Option<InventoryItem>.None();
        updatePos = false;
        rt.position = initPos.Get();
    }

}