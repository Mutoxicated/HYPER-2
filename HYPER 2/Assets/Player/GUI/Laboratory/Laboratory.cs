using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Laboratory : MonoBehaviour, IActivation
{
    [SerializeField] private PowerupManager powerupManager;
    [SerializeField] private GameObject labItemPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private Transform anchor;
    [SerializeField] private TMP_Text noItems;
    [SerializeField] private TMP_Text researchSlotCost;
    [SerializeField] private TMP_Text slots;

    private List<LabItem> labItems = new List<LabItem>();

    private List<LabItem> currentlyResearchedLabItems = new List<LabItem>();
    private float yInc;
    private string ufmtSlots;

    private Arsenal arsenal;
    private Researcher researcher;
    private PowerupPopup popup;
    private GUI gui;

    public void DStart(Arsenal arsenal, GUI gui, PowerupPopup popup, Researcher researcher) {
        this.researcher = researcher;
        this.popup = popup;
        this.gui = gui;
        this.arsenal = arsenal;

        var enumValues = Enum.GetValues(typeof(Resources.Arsenal)).ConvertTo<Resources.Arsenal[]>();

        for (int i = 0; i < enumValues.Length; i++) {
            var instance = Instantiate(labItemPrefab, container);
            var labItem = instance.GetComponent<LabItem>();
            labItems.Add(labItem);
            labItem.CStart(researcher, popup, enumValues[i], this, i);
        }
        yInc = labItems[0].ImageSizeY*60.1f;
        researchSlotCost.text = Gameplay.ResearchSlotCost+"*";
        ufmtSlots = slots.text;
    }

    public void Enable() {
        currentlyResearchedLabItems.Clear();
        for (int i = 0; i < arsenal.Keys.Count; i++) {
            if (arsenal.Keys[i].researchActive) {
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
        slots.text = String.Format(ufmtSlots, Save.rData.ReseachSlots);
        if (gui.Player.OnToggleLab) {
            if (popup.gameObject.activeInHierarchy) {
                popup.Disable();
            }else {
                gui.ToggleActivation(this);
            }
        }
    }

    public void BuyResearchSlot() {
        if (Save.rData.Money < Gameplay.ResearchSlotCost) {
            return;
        }
        if (Save.rData.ReseachSlots >= Gameplay.ResearchSlotLimit) {
            return;
        }
        Player.self.SetMoneyRelative(-Gameplay.ResearchSlotCost);
        researcher.IncResearchSlots();
    }
}
