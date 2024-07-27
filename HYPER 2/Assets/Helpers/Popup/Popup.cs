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

    public void CStart(Transform player) {
        this.player = player;
    }

    public void Action(Vector3 position, Color color, string text, float duration = 1f, float dy = 0.2f) {
        transform.position = position;
        this.text.text = text;
        this.duration = duration;
        this.dy = dy;
        this.color = color;
        this.text.color = color;
        enabled = true;
    }

    public void Update() {
        time += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, transform.position+Vector3.up*dy, Time.deltaTime);
        look = Quaternion.LookRotation(player.position-transform.position);
        transform.rotation = look;
        color.a = time / duration;
        text.color = color;
        if (time >= duration) {
            ppp.RetrievePopup(this);
            enabled = false;
        }
    }
}
