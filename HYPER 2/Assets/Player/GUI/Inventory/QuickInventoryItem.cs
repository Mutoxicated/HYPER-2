using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickInventoryItem : MonoBehaviour
{
    private const float radius = 0.1f;

    [SerializeField] private Inventory inventory;
    private Option<Resources.Arsenal> arsenalType = Option<Resources.Arsenal>.None();
    private Option<StatsTemplate> template = Option<StatsTemplate>.None();
    [SerializeField] private Image image;
    private int index;
    private QuickInventory quickInventory;

    public int Index {
        get { return index; }
    }

    public Option<Resources.Arsenal> ArsenalType {
        get { return arsenalType; }
    }

    public Option<StatsTemplate> Template {
        get { return template; }
    }

    public void CStart(QuickInventory qi, int index) {
        quickInventory = qi;
        this.index = index;
        gameObject.SetActive(true);
    }

    public void CheckCD() {
        inventory.currentlyDragged.GetAndThen(i => {
            if (Vector3.Distance(i.transform.position, transform.position) <= radius) {
                quickInventory.UpdateQI(index, i.ArsenalType);
            }
        });
    }

    public void SetArsenal(Option<Resources.Arsenal> arsenalType) {
        Save.rData.QIAs[index] = arsenalType;
        this.arsenalType = arsenalType;
        if (arsenalType.IsNone()) {
            image.sprite = Resources.Empty;
        }else {
            image.sprite = Resources.GetArsenalSprite(arsenalType.Get());
        }
    }
}
