using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private UnityEngine.InputSystem.PlayerInput input;
    private Manager manager;

    public UnityEngine.InputSystem.PlayerInput Input {
        get { return input; }
    }

    public Manager Manager {
        get { return manager; }
    }

    void Awake()
    {
        input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();
    }
}
