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
    [SerializeField] private Inventory inventory;
    [SerializeField] private PopupPool popupPool;
    [SerializeField] private GameObject disabableUI;

    [Header("Menu UI")]
    [SerializeField] private GameObject menuUI;

    public Color UIColor {
        get { return Save.pData.UIColor; }
    }
    public PopupPool PopupPool {
        get { return popupPool; }
    }

    private Player player;

    public void DStart(Player player) {
        this.player = player;
        selector.DStart(player, player.Manager);
        quickInventory.DStart(player);
        inventory.DStart();
        arsenal.DStart(player);
        popupPool.DStart(player.gameObject);
        gameObject.SetActive(true);
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

    private void Update() {
        if (player.OnDisableUI) {
            disabableUI.SetActive(!disabableUI.activeInHierarchy);
        }
    }
}
