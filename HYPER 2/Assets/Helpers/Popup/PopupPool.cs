using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPool : MonoBehaviour
{
    [SerializeField] private GameObject popupPrefab;

    private List<Popup> actives = new List<Popup>();
    private List<Popup> inactives = new List<Popup>();

    public void Popup(Vector3 position, Color color, string text, float duration = 1f, float dy = 0.2f) {
        if (inactives.Count == 0) {
            var instance = Instantiate(popupPrefab, transform);
            var popup = instance.GetComponent<Popup>();
            actives.Add(popup);
            popup.Action(position, color, text, duration, dy);
        }else {
            var popup = inactives[0];
            popup.Action(position, color, text, duration, dy);
            inactives.Remove(popup);
        }
    }

    public void RetrievePopup(Popup popup) {
        actives.Remove(popup);
        inactives.Add(popup);
    }
}
