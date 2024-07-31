using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private Option<AoEVFX> test1;

    public void DStart() {
        test1.GetAndThen(a => a.CStart());
    }
}
