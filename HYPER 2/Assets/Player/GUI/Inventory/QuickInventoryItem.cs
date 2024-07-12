using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickInventoryItem : MonoBehaviour
{
    private Option<Resources.Arsenal> arsenalType = Option<Resources.Arsenal>.None();
    private Option<StatsTemplate> template = Option<StatsTemplate>.None();
    [SerializeField] private Image image;
    private int index;

    public int Index {
        get { return index; }
    }

    public Option<Resources.Arsenal> ArsenalType {
        get { return arsenalType; }
    }

    public Option<StatsTemplate> Template {
        get { return template; }
    }

    public void CStart(int index) {
        this.index = index;
        gameObject.SetActive(true);
    }

    public void SetArsenal(Option<Resources.Arsenal> arsenalType) {
        this.arsenalType = arsenalType;
        if (arsenalType.IsNone()) {
            image.sprite = Resources.Empty;
        }else {
            image.sprite = Resources.GetArsenalSprite(arsenalType.Get());
        }
    }

    public void SetTemplate(Option<StatsTemplate> template) {
        this.template = template;
    }
}
