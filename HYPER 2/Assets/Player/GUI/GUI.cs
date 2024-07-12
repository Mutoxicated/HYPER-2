using UnityEngine;

public class GUI : MonoBehaviour
{
    [Header("Death UI")]
    [SerializeField] private GameObject deathUI;

    [Header("Playing UI")]
    [SerializeField] private GameObject playingUI;
    [SerializeField] private QuickInventory quickInventory;
    [SerializeField] private Selector selector;
    [SerializeField] private Arsenal arsenal;

    [Header("Menu UI")]
    [SerializeField] private GameObject menuUI;

    public Color UIColor {
        get { return Save.pData.UIColor; }
    }

    private Player player;

    public void DStart(Player player) {
        this.player = player;
        selector.DStart(player, player.Manager);
        quickInventory.DStart();
        arsenal.CStart();
    }

    public void UIMode(bool freezeTime) {
        player.UIMode(freezeTime);
        selector.gameObject.SetActive(false);
    }

    public void PlayingMode() {
        player.PlayingMode();
        selector.gameObject.SetActive(true);
    }

    public void DeadUI() {
        deathUI.SetActive(true);
        playingUI.SetActive(false);
    }
}
