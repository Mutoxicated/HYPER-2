using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Save save;
    [SerializeField] private Seed seed;
    [SerializeField] private Button continueRun;
    [SerializeField] private SceneSwitcher ss;
    [SerializeField] private GameObject menu;
    [SerializeField] private Settings settings;
    [SerializeField] private PlayerGraphics graphix;
    [SerializeField] private SoundManager sm;

    private void Start() {
        save.DStart();
        sm.DStart();
        graphix.DStart();
        settings.DStart(() => {
            menu.SetActive(true);
        }, () => {
            sm.Refresh();
            graphix.Refresh();
        });
        if (Save.rData == null) {
            continueRun.Disable();
        }
    }

    private void SetSeed() {
        seed.SetSeed();
    }

    public void ContinueRun() {
        SetSeed();
        ss.MainToArena();
    }

    public void NewRun() {
        Save.CreateData(DataType.RUN);
        SetSeed();
        ss.MainToArena();
    }

    public void OpenSettings() {
        settings.Enable();
        menu.SetActive(false);
    }
}
