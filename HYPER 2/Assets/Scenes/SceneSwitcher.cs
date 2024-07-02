using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void MainToArena() {
        SceneManager.LoadScene("Arena");
    }

    public void ArenaToMain() {
        SceneManager.LoadScene("MainMenu");
    }
}
