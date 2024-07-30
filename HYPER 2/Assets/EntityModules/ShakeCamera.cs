using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField] private float strength = 1f;

    private void OnEnable() {
        Player.self.ShakeCamera(transform.position, strength);
    }
}
