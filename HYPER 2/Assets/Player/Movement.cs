using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField, Range(1f,10f)] private float speed;

    private void OnMove(InputSystem.CallbackContext callbackContext) {

    }

}
