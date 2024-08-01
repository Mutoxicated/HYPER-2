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

    private StatsTemplate template;
    private bool selected;

    public StatsTemplate Template {
        get { return template; }
    }

    public bool BeingResearched {
        get { return beingResearched; }
    }

    private bool locked;

    public bool Locked {
        get { return locked; }
    }

    public void SetResearched(bool value) {
        beingResearched = value;
        SetImageColor();
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
        template = Resources.GetStatTemplate(Type);
        if (Save.rData[arsenalType, 0]) {
            SetResearched(true);
        }
        if (Save.rData.Lvl >= template.unlockAt) {
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
        if (Save.rData.Lvl >= Template.unlockAt) {
            locked = false;
            internalImage.sprite = Resources.GetArsenalSprite(type);
        }else {
            locked = true;
            internalImage.sprite = Resources.Locked;
        }
    }

    public void SetImageColor() {
        if (beingResearched) {
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
        if (Template.unlockAt > other.Template.unlockAt) {
            return 1;
        }else if (Template.unlockAt < other.Template.unlockAt) {
            return -1;
        }

        return 0;
    }
}
