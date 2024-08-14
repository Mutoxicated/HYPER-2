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
    [SerializeField] private Laboratory laboratory;
    [SerializeField] private PopupPool popupPool;
    [SerializeField] private HPBarPool hpbp;
    [SerializeField] private GameObject disabableUI;
    [SerializeField] private Notification notification;

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

    private Option<IActivation> currentlyOpen = Option<IActivation>.None();

    public void DStart(Player player) {
        this.player = player;
        notification.CStart();
        menuUI.DStart(player);
        selector.DStart(player, player.Manager);
        quickInventory.DStart(player);
        arsenal.DStart(player);
        inventory.DStart();
        laboratory.DStart();
        popupPool.DStart(player.gameObject);
        gameObject.SetActive(true);
    }

    public void Toggle(int type) {
        switch (type) {
        case 0:
            ToggleActivation(arsenal);
            break;
        case 1:
            ToggleActivation(inventory);
            break;
        case 2:
            ToggleActivation(laboratory);
            break;
        }
    }

    public void ToggleActivation(IActivation activation) {
        if (!activation.State())
            OpenActivation(activation);
        else CloseActivation();
    }

    public void OpenActivation(IActivation activation) {
        CloseActivation();
        activation.Enable();
        UIMode(false);
        currentlyOpen = new Option<IActivation>(activation);
    }

    public bool CurrentlyOpen() {
        return currentlyOpen.IsSome();
    }

    public void CloseActivation() {
        currentlyOpen.GetAndThen(c => {
            c.Disable();    
            PlayingMode();
            currentlyOpen = Option<IActivation>.None();
        });
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

    public void PushNotif(string msg) {
        notification.Enable(msg);
    }

    private void Update() {
        if (player.OnDisableUI) {
            disabableUI.SetActive(!disabableUI.activeInHierarchy);
        }
        if (player.OnToggleArsenal) {
            ToggleActivation(arsenal);
        }
        if (player.OnToggleInventory) {
            ToggleActivation(inventory);
        }
        if (player.OnToggleLab) {
            ToggleActivation(laboratory);
        }
        if (player.OnQuit) {
            if (CurrentlyOpen()) {
                CloseActivation();
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
