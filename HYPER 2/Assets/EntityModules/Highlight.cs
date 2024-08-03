using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] private Transform rangeTransform;
    [Tooltip("Have this component disabled")]
    [SerializeField] private List<Outline> outlines;

    [SerializeField] private MeshRenderer range;

    public void Enable() {
        enabled = true;
        foreach (var ol in outlines)
            ol.enabled = true;
        range.enabled = true;
    }

    private void Update() {
        transform.localScale = new Vector3(rangeTransform.localScale.x, transform.localScale.y, rangeTransform.localScale.z);
    }

    public void Disable() {
        enabled = false;
          foreach (var ol in outlines)
            ol.enabled = false;
        range.enabled = false;
    }
}
