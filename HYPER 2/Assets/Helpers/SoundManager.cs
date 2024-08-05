using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public void DStart() {
        Refresh();
    }

    public void Refresh() {
        AudioListener.volume = Save.pData.MasterVolume;
    }
}
