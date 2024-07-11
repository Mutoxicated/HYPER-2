using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArsenalItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private ArsenalInfo ai;
    [SerializeField] private Resources.Arsenal @Type;
    [SerializeField] private bool bought;
    private UnityEngine.UI.Button button;
    [SerializeField] private Image image;

    private StatsTemplate template;
    private bool selected;

    public StatsTemplate Template {
        get { return template; }
    }

    public bool Bought {
        get { return bought; }
    }

    public void OnSelect() {
        selected = false;
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
            ai.Enable(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
            ai.Disable();
    }

    private void Start() {
        template = Resources.GetStatTemplate(Type);
        if (bought) {
            image.color = Color.green;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        ai.Lock(this);
        selected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ai.Unlock();
        ai.Disable();
        selected = false;
    }
}
