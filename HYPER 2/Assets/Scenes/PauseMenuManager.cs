using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private GameObject menu;

    public void DStart(SoundManager sm, PlayerGraphics graphix) {
        settings.DStart(() => {
            menu.SetActive(true);
        }, () => {
            sm.Refresh();
            graphix.Refresh();
        });
    }

    public void OpenSettings() {
        settings.Enable();
        menu.SetActive(false);
    }
}
