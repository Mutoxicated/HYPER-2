using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Save save;
    [SerializeField] private Seed seed;
    [SerializeField] private Button continueRun;

    private void Start() {
        save.DStart();
        if (Save.rData == null) {
            continueRun.Disable();
        }
    }

    public void SetSeed() {
        seed.SetSeed();
    }
}
