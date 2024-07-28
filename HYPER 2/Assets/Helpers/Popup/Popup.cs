using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private PopupPool ppp;
    private Transform player;  
    private Quaternion look;

    private float time = 0;
    private float duration;
    private float dy;
    private Color color;
    private Vector3 initScale;

    public void CStart(PopupPool ppp, Transform player) {
        this.player = player;
        initScale = transform.localScale;
        this.ppp = ppp;
    }

    public void Action(Vector3 position, Color color, string text, float scale = 1f, float duration = 1.4f, float dy = 2f) {
        //Debug.Log("uhm?");
        transform.position = position;
        this.text.text = text;
        this.duration = duration;
        this.dy = dy;
        this.color = color;
        this.text.color = color;
        transform.localScale = initScale*scale;
        gameObject.SetActive(true);
    }

    public void Update() {
        time += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, transform.position+Vector3.up*dy, Time.deltaTime);
        look = Quaternion.LookRotation(player.position-transform.position);
        transform.rotation = look;
        color.a = Mathf.Abs(Mathf.Clamp01(time / (duration*0.85f))-1);
        text.color = color;
        if (time >= duration) {
            ppp.RetrievePopup(this);
            gameObject.SetActive(false);
            time = 0f;
        }
    }
}
