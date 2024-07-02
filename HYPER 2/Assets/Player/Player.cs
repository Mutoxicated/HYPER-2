using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private UnityEngine.InputSystem.PlayerInput input;
    private Manager manager;
    [SerializeField] private Movement movement;
    private Camera cam;
    private Transform camRoot;
    private InputAction debugRefresh;

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

    void Awake()
    {
        input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        input.actions.FindActionMap("Debug", true).Enable();
        debugRefresh = input.actions.FindAction("DRef");
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        camRoot = cam.transform.parent;
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();
        movement.DStart();
    }
}
