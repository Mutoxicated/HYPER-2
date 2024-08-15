using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image image;

    private PowerupPopup popup;
    private Resources.MTUpgradeType upgradeType;
    private MTUpgrade upgrade;
    private string desc;
    private string rarity;

    public void CStart(PowerupPopup popup) {
        this.popup = popup;
    }

    public void Enable(Resources.MTUpgradeType upgradeType, int rarity) {
        this.rarity = Rarity.FromPerc(rarity);
        this.upgradeType = upgradeType;

        upgrade = Resources.GetMTUpgrade(upgradeType);
        desc = upgrade.Description();

        image.sprite = Resources.GetMTUpgradeSprite(upgradeType);
        gameObject.SetActive(true);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        popup.ShowDescription(desc, rarity);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popup.ShowDescription("", "");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        popup.UpgradeTaken(upgradeType);
    }
}
