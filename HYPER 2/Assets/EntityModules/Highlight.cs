using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [Tooltip("Have this component disabled")]
    [SerializeField] private Outline outline;

    [SerializeField] private MeshRenderer range;

    public void Enable() {
        outline.enabled = true;
        range.enabled = true;
    }

    public void Disable() {
        outline.enabled = false;
        range.enabled = false;
    }
}
