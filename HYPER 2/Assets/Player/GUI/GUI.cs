using UnityEngine;

public class GUI : MonoBehaviour
{
    [Header("Death UI")]
    [SerializeField] private DeathScreen deathScreen;

    [Header("Playing UI")]
    [SerializeField] private GameObject playingUI;
    [SerializeField] private QuickInventory quickInventory;
    [SerializeField] private Selector selector;
    [SerializeField] private Arsenal arsenal;
    [SerializeField] private Inventory inventory;
    [SerializeField] private PopupPool popupPool;
    [SerializeField] private HPBarPool hpbp;
    [SerializeField] private GameObject disabableUI;

    [Header("Menu UI")]
    [SerializeField] private PauseMenuManager menuUI;

    public Color UIColor {
        get { return Save.pData.UIColor; }
    }
    public PopupPool PopupPool {
        get { return popupPool; }
    }
    
    public HPBarPool HPBP {
        get { return hpbp; }
    }

    private Player player;

    public void DStart(Player player) {
        this.player = player;
        menuUI.DStart(player);
        selector.DStart(player, player.Manager);
        quickInventory.DStart(player);
        arsenal.DStart(player);
        inventory.DStart();
        popupPool.DStart(player.gameObject);
        gameObject.SetActive(true);
    }

    public bool ArsenalActive() {
        return arsenal.gameObject.activeInHierarchy;
    }

    public void OpenArsenal() {
        if (inventory.gameObject.activeInHierarchy) {
            CloseInventory();
        }
        arsenal.Enable();
        UIMode(false);
    }

    public void CloseArsenal() {
        arsenal.Disable();
        PlayingMode();
    }

    public bool InventoryActive() {
        return inventory.gameObject.activeInHierarchy;
    }

    public void OpenInventory() {
        if (arsenal.gameObject.activeInHierarchy) {
            CloseArsenal();
        }
        inventory.Enable();
        UIMode(false);
    }

    public void CloseInventory() {
        inventory.Disable();
        PlayingMode();
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
        deathScreen.Enable();
        playingUI.SetActive(false);
    }

    private void Update() {
        if (player.OnDisableUI) {
            disabableUI.SetActive(!disabableUI.activeInHierarchy);
        }
        if (player.OnToggleArsenal) {
            if (!arsenal.gameObject.activeInHierarchy)
                OpenArsenal();
            else CloseArsenal();
        }
        if (player.OnToggleInventory) {
            if (!inventory.gameObject.activeInHierarchy)
                OpenInventory();
            else CloseInventory();
        }
        if (player.OnQuit) {
            if (InventoryActive()) {
                CloseInventory();
            }else if (ArsenalActive()) {
                CloseArsenal();
            }else {
                menuUI.gameObject.SetActive(!menuUI.gameObject.activeInHierarchy);
                playingUI.gameObject.SetActive(!playingUI.gameObject.activeInHierarchy);
                if (menuUI.gameObject.activeInHierarchy)
                    UIMode(true);
                else PlayingMode();
            }
        }
    }
}
