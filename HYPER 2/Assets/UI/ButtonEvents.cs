using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvents : MonoBehaviour
{
    public void NewRun() {
        Save.CreateData(DataType.RUN);
    }
    public void Quit() {
        Save.UpdateJsonData(DataType.RUN);
        Application.Quit();
    }
}
