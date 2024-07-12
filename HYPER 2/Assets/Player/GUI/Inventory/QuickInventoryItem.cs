using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInventoryItem : MonoBehaviour
{
    private Option<Resources.Arsenal> arsenalType = Option<Resources.Arsenal>.None();
    [SerializeField] private Image image;
    [SerializeField] private int index;

    public int Index {
        get { return index; }
    }

    public Option<Resources.Arsenal> ArsenalType {
        get { return arsenalType; }
    }

    public void CStart(int index) {
        this.index = index;
        image.sprite = Resources.Empty;
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
}
