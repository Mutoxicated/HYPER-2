using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private AoEVFX test1;

    public void DStart() {
        test1.CStart();
    }
}
