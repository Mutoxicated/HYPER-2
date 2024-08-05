using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvents : MonoBehaviour
{
    public void Quit() {
        Save.UpdateJsonData(DataType.RUN);
        Application.Quit();
    }
}
