using TMPro;
using UnityEngine;

public class Field : MonoBehaviour {
    [SerializeField] private TMP_Text fieldName;
    [SerializeField] private TMP_Text fieldValue;

    public void CStart() {
        gameObject.SetActive(true);
    }

    public void ChangeName(string name) {
        fieldName.text = name+":";
    }

    public void ChangeValue(object value) {
        fieldValue.text = value.ToString();
    }
}