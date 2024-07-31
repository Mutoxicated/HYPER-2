using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PopupPool : MonoBehaviour
{
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private GameObject player;

    private List<Popup> actives = new List<Popup>();
    private List<Popup> inactives = new List<Popup>();

    public void DStart(GameObject playerGo) {
        player = playerGo;
    }

    public void Popup(Vector3 position, Color color, string text, float scale = 1f, float duration = 1f, float dy = 0.2f) {
        Debug.Log("Happened");
        if (inactives.Count == 0) {
            var instance = Instantiate(popupPrefab, transform);
            var popup = instance.GetComponent<Popup>();
            actives.Add(popup);
            popup.CStart(this, player.transform);
            popup.Action(position, color, text, scale, duration, dy);
        }else {
            var popup = inactives[0];
            popup.Action(position, color, text, scale, duration, dy);
            inactives.Remove(popup);
        }
    }

    public void RetrievePopup(Popup popup) {
        actives.Remove(popup);
        inactives.Add(popup);
    }
}
