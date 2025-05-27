using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public enum Asker {
    ARSENAL,
    INVENTORY,
    PAUSE_MENU
}

public class Player : MonoBehaviour
{
    public static Player self;

    private UnityEngine.InputSystem.PlayerInput input;
    private Manager manager;
    [SerializeField] private Stats towerStats;
    [SerializeField] private Movement movement;
    [SerializeField] private GUI gui;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera mainCam;
    [SerializeField] private PostProcessVolume ppv;
    [SerializeField] private CameraShake camShake;
    [SerializeField] private MusicPlayer musicPlayer;
    [SerializeField] private PlayerGraphics graphix;
    private Transform camRoot;

    private InputAction debugRefresh;
    private InputAction debugPanel;
    private InputAction middleButton;
    private InputAction leftButton;
    private InputAction deleteButton;
    private InputAction uiDeleteButton;
    private InputAction disableUIButton;
    private InputAction tabButton;
    private InputAction inventoryButton;
    private InputAction labButton;
    private InputAction quitButton;

    private InputActionMap player;
    private InputActionMap ui;
    private InputActionMap debug;
    private InputActionMap common;

    private bool dead = false;

    public delegate void LvlUp(int lvl);
    public LvlUp onLvlUp = new LvlUp(lvl => {});

    public UnityEngine.InputSystem.PlayerInput Input {
        get { return input; }
    }

    public PlayerGraphics Graphix {
        get { return graphix; }
    }

    public Manager Manager {
        get { return manager; }
    }

    public World World {
        get { return manager.World; }
    }

    public Transform CamRoot {
        get { return camRoot; }
    }

    public Camera Cam {
        get { return cam; }
    }

    public Camera MainCam {
        get { return mainCam; }
    }

    public InputAction FindAction(string name) {
        return Input.actions.FindAction(name, true);
    }

    public bool OnUIDelete {
        get { return uiDeleteButton.WasPerformedThisFrame(); }
    }

    public bool OnDebugRefresh {
        get { return debugRefresh.WasPerformedThisFrame(); }
    }

    public bool OnDebugPanel {
        get { return debugPanel.WasPerformedThisFrame(); }
    }

    public bool OnMiddleButton {
        get { return middleButton.IsPressed(); }
    }

    public bool OnLeftClick {
        get { return leftButton.WasPerformedThisFrame(); }
    }

    public bool OnDelete {
        get { return deleteButton.WasPerformedThisFrame(); }
    }

    public bool OnDisableUI {
        get { return disableUIButton.WasPerformedThisFrame(); }
    }

    public bool OnToggleArsenal {
        get { return tabButton.WasPerformedThisFrame(); }
    }

    public bool OnToggleInventory {
        get { return inventoryButton.WasPerformedThisFrame(); }
    }

    public bool OnToggleLab {
        get { return labButton.WasPerformedThisFrame(); }
    }

    public bool OnQuit {
        get { return quitButton.WasPerformedThisFrame(); }
    }

    public bool Dead {
        get { return dead; }
    }
    public GUI GUI {
        get { return gui; }
    }

    public int Money {
        get { return money; }
    }
    
    public void AddLvlUpListener(LvlUp fn) {
        onLvlUp += fn;
    }

    private float XP;
    public float CurrentXP
    {
        get { return XP; }
    }
    private int lvl;
    public int Lvl
    {
        get { return lvl; }
    }
    private int money;

    public void IncrementXP(float val)
    {
        float newXP = XP + val;
        while (newXP >= MaxXPRequired())
        {
            newXP -= MaxXPRequired();
            lvl++;
            self.towerStats.LvlUp(true);
            money += Gameplay.MoneyEarned(lvl);
            self.onLvlUp.Invoke(lvl);
        }
        XP = newXP;
    }

    public void SetMoneyRelative(int val) {
        money += val;
    }

    public static float MaxXPRequired() {
        return Save.rData.Lvl*10f;
    }

    public void DeadMode() {
        manager.GP.gameObject.SetActive(false);
        dead = true;
        UIMode(false);
        gui.DeadUI();
        musicPlayer.Enable();
    }

    public void UIMode(bool freezeTime) {
        if (freezeTime)
            Time.timeScale = 0f;
        player.Disable();
        debug.Disable();
        ui.Enable();
        if (freezeTime)
            musicPlayer.Disable();
    }

    public void PlayingMode() {
        Time.timeScale = 1f;
        player.Enable();
        debug.Enable();
        ui.Disable();
        musicPlayer.Enable();
    }

    public void ShakeCamera(Vector3 caller, float strength = 1f) {
        camShake.Shake(caller, strength);
    }

    public void DStart(Manager manager) {
        self = this;
        money = Save.rData.Money;
        lvl = Save.rData.Lvl;
        XP = Save.rData.XP;
        PublicEvent.onSaveRun += _ =>
        {
            Save.rData.SetMoney(money);
            Save.rData.SetLvl(lvl);
            Save.rData.SetXP(XP);
        };
        input = GetComponent<UnityEngine.InputSystem.PlayerInput>();

        player = input.actions.FindActionMap("Player", true);
        middleButton = player.FindAction("MiddleButton", true);
        leftButton = player.FindAction("Fire");
        deleteButton = player.FindAction("Delete");
        disableUIButton = player.FindAction("DisableUI");

        debug = input.actions.FindActionMap("Debug", true);
        debug.Enable();
        debugPanel = debug.FindAction("Panel");
        debugRefresh = debug.FindAction("DRef");

        ui = input.actions.FindActionMap("UI", true);
        uiDeleteButton = ui.FindAction("Delete");

        common = input.actions.FindActionMap("Common", true);
        tabButton = common.FindAction("ToggleArsenal");
        inventoryButton = common.FindAction("ToggleInventory");
        labButton = common.FindAction("LabButton");
        quitButton = common.FindAction("Quit");
        common.Enable();

        PlayingMode();

        camRoot = cam.transform.parent;
        cam.fieldOfView = Save.pData.Fov;
        mainCam.fieldOfView = Save.pData.Fov;

        graphix.DStart();

        this.manager = manager;

        movement.DStart();
        gui.DStart(this);
        musicPlayer.DStart();

        gameObject.SetActive(true);
    }

    public void Refresh() {
        musicPlayer.Refresh();
        graphix.Refresh();
        manager.SM.Refresh();
        cam.fieldOfView = Save.pData.Fov;
        mainCam.fieldOfView = Save.pData.Fov;
    }
}
