using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform obj;

    [Header("Settings")]
    [SerializeField] private bool scalable = true;
    
    private const float lerpSpeed = 15f;
    private const float scaleMultiplier = 1.2f;
    private bool hovering;

    private Vector3 initScale;

    private void Start() {
        obj = obj == null? transform : obj;

        initScale = obj.localScale;
        if (!scalable) {
            enabled = false;
        }
    }

    public void Update() {
        if (hovering) {
            obj.localScale = Vector3.Lerp(obj.localScale, initScale*scaleMultiplier, Time.deltaTime*lerpSpeed);
        }else {
            obj.localScale = Vector3.Lerp(obj.localScale, initScale, Time.deltaTime*lerpSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
