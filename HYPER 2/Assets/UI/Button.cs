using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Transform obj;
    [SerializeField] private Image image;
    [SerializeField] private Image secondImage;
    [SerializeField] private Option<TMP_Text> text;

    [Header("Settings")]
    [SerializeField] private bool scalable = true;
    [SerializeField] private bool playSound = false;
    
    private const float lerpSpeed = 15f;
    private const float scaleMultiplier = 1.2f;
    private bool hovering;
    private Option<Color> color = Option<Color>.None();

    private Vector3 initScale;

    public void Disable() {
        obj = obj == null? transform : obj;
        enabled = false;
        obj.localScale = initScale;
        text.GetAndThen(t => {
            t.color = Color.grey; 
        });
    }

    public void Enable() {
        enabled = true;
        text.GetAndThen(t => {
            t.color = color.Get(); 
        });
    }

    private void Start() {
        text.GetAndThen(t => {
            color = new Option<Color>(t.color);
        });
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
            obj.localScale = Vector3.Lerp(obj.localScale, initScale*scaleMultiplier, Time.unscaledDeltaTime*lerpSpeed);
        }else {
            obj.localScale = Vector3.Lerp(obj.localScale, initScale, Time.unscaledDeltaTime*lerpSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playSound)
            SoundCollection.Play(Audio.BOOP);
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playSound)
            SoundCollection.Play(Audio.BEEP);
    }
}
