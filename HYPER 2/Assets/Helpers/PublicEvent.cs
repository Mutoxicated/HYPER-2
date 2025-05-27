using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PublicEvent : MonoBehaviour
{
    public delegate void OnSaveRun(RunData data);

    public static OnSaveRun onSaveRun = new OnSaveRun(r => {});

    public static void SaveRun()
    {
        onSaveRun.Invoke(Save.rData);
        Save.UpdateJsonData(DataType.RUN);
    }

    public static void ClearAll() {
        onSaveRun = new OnSaveRun(r => {});
    }
}
