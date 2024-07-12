using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler {
    private Resources.Arsenal arsenalType;
    [SerializeField] private Image image;
    private bool updatePos;
    private Vector3 mousePos;
    private Vector3 initPos;

    private Vector3 alteredPos;
    private void UpdatePos(Vector3 wPos){
        alteredPos = wPos;
        alteredPos.x = transform.position.x;
        transform.position = alteredPos;
    }

    private void Update() {
        if (!updatePos) return;
        mousePos = Mouse.current.position.ReadValue();
        //mousePos.z = (transform.position.x-Camera.main.transform.position.z)*0.07f;
        UpdatePos(Camera.main.ScreenToWorldPoint(mousePos));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (initPos == null) {
            initPos = transform.position;
        }
        updatePos = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        updatePos = false;
        transform.position = initPos;
    }
}