using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArsenalItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IComparable<ArsenalItem>
{
    [SerializeField] private ArsenalInfo ai;
    [SerializeField] private Resources.Arsenal @type;
    [SerializeField] private bool beingResearched;
    [SerializeField] private Image image;
    [SerializeField] private Image internalImage;
    [SerializeField] private Text itemName;

    private MinitowerInfo mtInfo;
    private StatsTemplate template;
    private bool selected;

    public MinitowerInfo MTInfo {
        get { return mtInfo; }
    }

    public StatsTemplate Template {
        get { return template; }
    }

    private bool locked;

    public bool Locked {
        get { return locked; }
    }

    public Resources.Arsenal Type {
        get { return @type; }
    }

    public void OnSelect() {
        selected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
            ai.Enable(this, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
            ai.Disable();
    }

    public void CStart(ArsenalInfo ai, Resources.Arsenal arsenalType) {
        type = arsenalType;
        mtInfo = Resources.GetMTInfo(Type);
        template = Resources.GetStatTemplate(Type);
        if (Save.rData.Lvl >= mtInfo.unlockAt) {
            ai.Arsenal.Inventory.NewInventoryItem(arsenalType);
        }
        this.ai = ai;
    }

    public void Enable() {
        internalImage.sprite = Resources.GetArsenalSprite(type);
        SetImageColor();
        UpdateLockState();
        gameObject.SetActive(true);
    }

    public void UpdateLockState() {
        if (Save.rData.Lvl >= mtInfo.unlockAt) {
            locked = false;
            internalImage.sprite = Resources.GetArsenalSprite(type);
        }else {
            locked = true;
            internalImage.sprite = Resources.Locked;
        }
    }

    public void SetImageColor() {
        if (Save.rData[type, 0]) {
            image.color = Color.cyan;
        }else if (!locked) {
            image.color = Color.white;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        ai.Enable(this, true);
        selected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ai.Unlock();
        selected = false;
    }

    public int CompareTo(ArsenalItem other)
    {
        if (mtInfo.unlockAt > other.mtInfo.unlockAt) {
            return 1;
        }else if (mtInfo.unlockAt < other.mtInfo.unlockAt) {
            return -1;
        }

        return 0;
    }
}
