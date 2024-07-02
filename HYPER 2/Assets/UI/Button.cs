using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform obj;
    [SerializeField] private Image image;
    [SerializeField] private Image secondImage;

    [Header("Settings")]
    [SerializeField] private bool scalable = true;
    
    private const float lerpSpeed = 15f;
    private const float scaleMultiplier = 1.2f;
    private bool hovering;

    private Vector3 initScale;

    private void Start() {
        obj = obj == null? transform : obj;
        if (image == null && secondImage != null) {
            image = GetComponent<Image>();
        }

        initScale = obj.localScale;
        if (!scalable) {
            enabled = false;
        }
    }

    public void Update() {
        if (secondImage != null) {
            secondImage.color = image.color;
        }
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
