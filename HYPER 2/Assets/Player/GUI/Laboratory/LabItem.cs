using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabItem : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private Image progressBar;
    [SerializeField] private TMP_Text lvl;
    [SerializeField] private PowerupMono powerup;

    private Resources.Arsenal arsenalType;
    private StatsTemplate template;
    private Laboratory lab;
    private string ufmtLvl;

    public float ImageSizeY {
        get { return image.transform.localScale.y; }
    }

    public void CStart(PowerupPopup popup, Resources.Arsenal arsenalType, Laboratory lab, int index) {
        this.lab = lab;
        template = Resources.GetStatTemplate(arsenalType);
        this.arsenalType = arsenalType;
        image.sprite = Resources.GetArsenalSprite(arsenalType);
        ufmtLvl = lvl.text;
        powerup.CStart(popup, arsenalType, index);
    }

    public void Enable() {
        Update();
        powerup.Enable();
        gameObject.SetActive(true);
    }

    public void Disable() {
        powerup.Disable();
        gameObject.SetActive(false);
    }

    private void Update() {
        progressBar.fillAmount = Save.rData[arsenalType]/template.maxXp;
        lvl.text = String.Format(ufmtLvl, Save.rData[arsenalType, false]);
    }

    public void StopResearching() {
        Save.rData.SetCurrentlyResearched(arsenalType, false);
        lab.Disable();
        lab.Enable();
    }
}