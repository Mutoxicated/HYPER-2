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

    private float rotationX, rotationY;
    private Quaternion lookRotation;

    private const float xRotationLimit = 85;

    private void Start() {
        lookAccess = player.FindAction("LookAccess");
        look = player.FindAction("Look");

        lookRotation = transform.rotation;
    }

    private void FixedUpdate() {
        if (!lookAccess.IsPressed()) {
            Cursor.lockState = CursorLockMode.None;
            return;
        };
        Cursor.lockState = CursorLockMode.Locked;

        moveVector = look.ReadValue<Vector2>();

        rotationX -= moveVector.y * Time.fixedDeltaTime * (Save.pData.SensY * 100f);
        rotationY += moveVector.x * Time.fixedDeltaTime * (Save.pData.SensX * 100f);
        //clamp it so that cam cant freely move
        rotationX = Mathf.Clamp(rotationX, -xRotationLimit, xRotationLimit);
        lookRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.fixedDeltaTime*37f);
    }
}
