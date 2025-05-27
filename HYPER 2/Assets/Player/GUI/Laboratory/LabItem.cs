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
    private Researcher researcher;
    private Laboratory lab;
    private string ufmtLvl;

    public float ImageSizeY {
        get { return image.transform.localScale.y; }
    }

    public void CStart(Researcher researcher, PowerupPopup popup, Resources.Arsenal arsenalType, Laboratory lab, int index) {
        this.researcher = researcher;
        this.lab = lab;
        template = Resources.GetStatTemplate(arsenalType);
        this.arsenalType = arsenalType;
        image.sprite = Resources.GetArsenalSprite(arsenalType);
        ufmtLvl = lvl.text;
        powerup.CStart(popup, arsenalType, index);
    }

    public void Enable() {
        Update();
        gameObject.SetActive(true);
    }

    public void Disable() {
        powerup.Disable();
        gameObject.SetActive(false);
    }

    private void Update() {
        progressBar.fillAmount = researcher.GetResearchValue(arsenalType)/template.maxXp;
        lvl.text = String.Format(ufmtLvl, researcher.GetResearchLvl(arsenalType));
    }

    public void StopResearching() {
        researcher.StopResearchingItem(arsenalType);
        lab.Disable();
        lab.Enable();
    }
}