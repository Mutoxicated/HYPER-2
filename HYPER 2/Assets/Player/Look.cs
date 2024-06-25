using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

public class Look : MonoBehaviour
{
    [SerializeField] private Player player;

    private InputAction lookAccess;
    private InputAction look;

    private Vector2 moveVector;

    private const float xRotationLimit = 85;

    private void Start() {
        lookAccess = player.FindAction("LookAccess");
        look = player.FindAction("Look");
    }

    private void Update() {
        if (!lookAccess.IsPressed()) {
            Cursor.lockState = CursorLockMode.None;
            return;
        };
        Cursor.lockState = CursorLockMode.Locked;

        moveVector = look.ReadValue<Vector2>();

        if (moveVector == Vector2.zero) {
            return;
        }

        var delta = new Vector3(-moveVector.y*4f*Save.pData.SensX, moveVector.x*4f*Save.pData.SensY, 0);
        var eulerAngles = player.Cam.transform.eulerAngles;

        player.Cam.transform.eulerAngles = Vector3.Lerp(eulerAngles, eulerAngles+delta, Time.deltaTime);
        var clampedX = player.Cam.transform.eulerAngles.x;
    
        if (clampedX > xRotationLimit && clampedX < 90f) {
            clampedX = xRotationLimit;
        }else if (clampedX < 360-xRotationLimit && clampedX > 90f) {
            clampedX = 360-xRotationLimit;
        }

        eulerAngles = player.Cam.transform.eulerAngles;
        player.Cam.transform.eulerAngles = new Vector3(clampedX, eulerAngles.y, eulerAngles.z);
    }
}
