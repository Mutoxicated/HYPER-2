using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PublicEvent : MonoBehaviour
{
    public delegate void OnSaveRun(RunData data);

    private static OnSaveRun saveRun = new OnSaveRun(r => {
        if (Player.self.Dead) {
            Save.RemoveData(DataType.RUN);
            return;
        }
        onSaveRun.Invoke(r);
        Save.UpdateJsonData(DataType.RUN);
    });
    public static OnSaveRun onSaveRun = new OnSaveRun(r => {});

    public static void SaveRun() {
        Save.rData.MTs.Clear();
        saveRun.Invoke(Save.rData);
    }

    public static void ClearAll() {
        onSaveRun = new OnSaveRun(r => {});
    }
}
