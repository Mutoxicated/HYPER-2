using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private UnityEngine.InputSystem.PlayerInput input;
    private Manager manager;
    [SerializeField] private Movement movement;
    [SerializeField] private GUI gui;
    [SerializeField] private GameObject[] miniTowerPrefabs;
    private Camera cam;
    private Transform camRoot;
    private InputAction debugRefresh;
    private InputAction middleButton;
    private InputAction leftButton;

    private InputActionMap player;
    private InputActionMap ui;
    private InputActionMap debug;
    

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

    public bool OnDebugRefresh {
        get { return debugRefresh.WasPerformedThisFrame(); }
    }

    public bool OnMiddleButton {
        get { return middleButton.IsPressed(); }
    }

    public bool OnLeftClick {
        get { return leftButton.WasPerformedThisFrame(); }
    }
    
    public void DeadMode() {
        UIMode();
        gui.DeadUI();        
    }

    public void UIMode() {
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

    public delegate void MiniTowerConstructor(MiniTower mini);

    public MiniTower InstantiateMiniTower(Vector3 position, int index, MiniTowerConstructor constructor) {
        var tower = Instantiate(miniTowerPrefabs[index], position, Quaternion.identity).GetComponent<MiniTower>();
        constructor(tower);
        return tower;
    }

    void Awake() {
        input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        player = input.actions.FindActionMap("Player", true);
        middleButton = player.FindAction("MiddleButton", true);
        leftButton = player.FindAction("Fire");
        debug = input.actions.FindActionMap("Debug", true);
        debug.Enable();
        ui = input.actions.FindActionMap("UI", true);
        debugRefresh = input.actions.FindAction("DRef");

        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        camRoot = cam.transform.parent;

        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();

        movement.DStart();
        gui.DStart(this);
    }
}
