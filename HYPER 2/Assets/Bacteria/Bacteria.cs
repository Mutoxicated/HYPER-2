using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BacteriaType {
    Fire
}

public class Bacteria : MonoBehaviour
{
    [SerializeField] private BacteriaType bt;

    public UnityAction onPopulationIncrease = new UnityAction(() => {});
    public UnityAction onPopulationDecrease = new UnityAction(() => {});

    public BacteriaType Type {
        get { return bt; }
    }

    
}
