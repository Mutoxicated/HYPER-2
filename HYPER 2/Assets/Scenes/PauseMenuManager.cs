using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private GameObject menu;

    public void DStart(Player player) {
        settings.DStart(() => {
            menu.SetActive(true);
        }, () => {
            player.Refresh();
        });
    }

    public void OpenSettings() {
        settings.Enable();
        menu.SetActive(false);
    }
}
