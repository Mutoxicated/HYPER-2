using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Laboratory : MonoBehaviour, IActivation
{
    [SerializeField] private GameObject labItemPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private Transform anchor;
    [SerializeField] private TMP_Text noItems;

    private List<LabItem> labItems = new List<LabItem>();

    private List<LabItem> currentlyResearchedLabItems = new List<LabItem>();
    private float yInc;

    private PowerupPopup popup;
    private GUI gui;

    public void DStart(GUI gui, PowerupPopup popup) {
        this.popup = popup;
        this.gui = gui;

        var enumValues = Enum.GetValues(typeof(Resources.Arsenal)).ConvertTo<Resources.Arsenal[]>();

        for (int i = 0; i < enumValues.Length; i++) {
            var instance = Instantiate(labItemPrefab, container);
            var labItem = instance.GetComponent<LabItem>();
            labItems.Add(labItem);
            labItem.CStart(popup, enumValues[i], this, i);
        }
        yInc = labItems[0].ImageSizeY*60.1f;
    }

    public void Enable() {
        currentlyResearchedLabItems.Clear();
        for (int i = 0; i < Save.rData.CurrentlyResearchingCount; i++) {
            if (Save.rData.CurrentlyResearching[i].active) {
                labItems[i].Enable();
                currentlyResearchedLabItems.Add(labItems[i]);
            }
        }
        noItems.gameObject.SetActive(currentlyResearchedLabItems.Count == 0);

        Sort();
        gameObject.SetActive(true);
    }

    public void Disable() {
        foreach (var li in currentlyResearchedLabItems) {
            li.Disable();
        }
        gameObject.SetActive(false);
    }

    private void Sort() {
        Vector3 rightDown = new Vector3(0.8f, -1, 0);
        Vector3 alteredPos;
        for (int i = 0; i < currentlyResearchedLabItems.Count; i++) {
            currentlyResearchedLabItems[i].transform.position = anchor.position;
            alteredPos = currentlyResearchedLabItems[i].transform.localPosition;

            alteredPos += rightDown*yInc*0.5f;
            alteredPos += Vector3.down*yInc*2f*i;

            currentlyResearchedLabItems[i].transform.localPosition = alteredPos;
        }
    }

    public bool State()
    {
        return gameObject.activeInHierarchy;
    }

    public void Update() {
        if (gui.Player.OnToggleLab) {
            if (popup.gameObject.activeInHierarchy) {
                popup.Disable();
            }else {
                gui.ToggleActivation(this);
            }
        }
    }
}
