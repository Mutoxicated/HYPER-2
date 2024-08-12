using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laboratory : MonoBehaviour, IActivation
{
    [SerializeField] private GameObject labItemPrefab;
    [SerializeField] private Transform anchor;

    private List<LabItem> labItems = new List<LabItem>();

    private List<LabItem> currentlyResearchedLabItems = new List<LabItem>();
    private float yInc;

    public void DStart() {
        var enumValues = Enum.GetValues(typeof(Resources.Arsenal)).ConvertTo<Resources.Arsenal[]>();

        for (int i = 0; i < enumValues.Length; i++) {
            var instance = Instantiate(labItemPrefab);
            var labItem = instance.GetComponent<LabItem>();
            labItems.Add(labItem);
            labItem.CStart(enumValues[i], this, i);
        }
        yInc = labItems[0].ImageSizeY*1.1f;
    }

    public void Enable() {
        currentlyResearchedLabItems.Clear();
        for (int i = 0; i < Save.rData.CurrentlyResearchingCount; i++) {
            if (Save.rData.CurrentlyResearching[i].Item1) {
                currentlyResearchedLabItems.Add(labItems[i]);
            }
        }

        Sort();
    }

    public void Disable() {
        gameObject.SetActive(false);
    }

    private void Sort() {
        Vector3 rightDown = new Vector3(1, -1);
        for (int i = 0; i < currentlyResearchedLabItems.Count; i++) {
            currentlyResearchedLabItems[i].transform.position = anchor.position+rightDown*yInc*0.5f+Vector3.down*yInc*i;
        }
    }
}
