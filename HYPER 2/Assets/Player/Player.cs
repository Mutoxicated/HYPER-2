using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player self;

    private UnityEngine.InputSystem.PlayerInput input;
    private Manager manager;
    [SerializeField] private Movement movement;
    [SerializeField] private GUI gui;
    [SerializeField] private Camera cam;
    private Transform camRoot;
    private InputAction debugRefresh;
    private InputAction middleButton;
    private InputAction leftButton;
    private InputAction deleteButton;
    private InputAction uiDeleteButton;

    private InputActionMap player;
    private InputActionMap ui;
    private InputActionMap debug;

    private bool dead = false;

    public delegate void LvlUp(int lvl);
    public LvlUp onLvlUp = new LvlUp(lvl => {});

    public UnityEngine.InputSystem.PlayerInput Input {
        get { return input; }
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

    public InputAction FindAction(string name) {
        return Input.actions.FindAction(name, true);
    }

    public bool OnUIDelete {
        get { return uiDeleteButton.WasPerformedThisFrame(); }
    }

    public bool OnDebugRefresh {
        get { return debugRefresh.WasPerformedThisFrame(); }
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

    public bool Dead {
        get { return dead; }
    }
    
    public void AddLvlUpListener(LvlUp fn) {
        onLvlUp += fn;
    }

    public static void IncrementXP(float val) {
        float newXP = Save.rData.XP+val;
        if (newXP >= MaxXPRequired()) {
            Save.rData.SetXP(0);
            Save.rData.SetLvl(Save.rData.Lvl+1);
            self.onLvlUp.Invoke(Save.rData.Lvl);
            Save.rData.SetMoneyRelative(Mathf.CeilToInt(Mathf.Sqrt(Save.rData.Lvl))*10);
            return;
        }
        Save.rData.SetXP(newXP);
    }

    public static void SetMoneyRelative(int val) {
        Save.rData.SetMoneyRelative(val);
    }

    public static float MaxXPRequired() {
        return Save.rData.Lvl*10f;
    }

    public void DeadMode() {
        dead = true;
        UIMode(true);
        gui.DeadUI();
    }

    public void UIMode(bool freezeTime) {
        if (freezeTime)
            Time.timeScale = 0f;
        player.Disable();
        debug.Disable();
        ui.Enable();
    }

    public void PlayingMode() {
        Time.timeScale = 1f;
        player.Enable();
        debug.Enable();
        ui.Disable();
    }

    public void DStart(Manager manager) {
        self = this;
        input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        player = input.actions.FindActionMap("Player", true);
        middleButton = player.FindAction("MiddleButton", true);
        leftButton = player.FindAction("Fire");
        deleteButton = player.FindAction("Delete");
        debug = input.actions.FindActionMap("Debug", true);
        debug.Enable();
        ui = input.actions.FindActionMap("UI", true);
        uiDeleteButton = ui.FindAction("Delete");
        debugRefresh = input.actions.FindAction("DRef");

        PlayingMode();

        camRoot = cam.transform.parent;

        this.manager = manager;

        movement.DStart();
        gui.DStart(this);

        gameObject.SetActive(true);
    }
}
