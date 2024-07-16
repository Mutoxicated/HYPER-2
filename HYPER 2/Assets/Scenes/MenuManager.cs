using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Save save;
    [SerializeField] private Seed seed;

    private void Start() {
        save.DStart();
        seed.DStart();
    }
}
