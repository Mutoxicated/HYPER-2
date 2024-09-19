using System;
using System.IO;
using Helpers;
using UnityEngine;

public enum DataType {
    RUN,
    PLAYER
}

public class Save : MonoBehaviour
{
    public static PlayerData pData = new PlayerData();
    public static RunData rData;

    public static string Path(DataType dt)
    {
        if (dt == DataType.RUN) {
            return Application.dataPath + System.IO.Path.AltDirectorySeparatorChar + "RunData.json";
        }
        return Application.dataPath + System.IO.Path.AltDirectorySeparatorChar + "PlayerData.json";
    }

    public DataType Type()
    {
        return DataType.PLAYER;
    }

    public void DStart()
    {
        if (File.Exists(Path(DataType.RUN)))
        {
            UpdateData(DataType.RUN);
        }
        
        if (File.Exists(Path(DataType.PLAYER)))
        {
            UpdateData(DataType.PLAYER);
        }else {
            CreateData(DataType.PLAYER);
        }
    }

    public static void CreateData(DataType dt)
    {
        if (dt == DataType.RUN) {
            rData = new RunData();
        }else {
            pData = new PlayerData();
        }
        UpdateJsonData(dt);
    }

    public static void UpdateJsonData(DataType dt)
    {
        string savePath = Path(dt);

        Debug.Log("Saving data to path: "+ savePath);

        string json = dt == DataType.RUN? JsonUtility.ToJson(rData) : JsonUtility.ToJson(pData);
        Debug.Log(json);

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
    }

    public static void UpdateData(DataType dt)
    {
        Debug.Log("Reading data");
        using StreamReader reader = new StreamReader(Path(dt));
        string json = reader.ReadToEnd();

        if (dt == DataType.RUN) {
            rData = JsonUtility.FromJson<RunData>(json);
            rData.InitArsenalLists();
        }else {
            pData = JsonUtility.FromJson<PlayerData>(json);
        }
    }

    public static void RemoveData(DataType dt){
        var path = Path(dt);
        if (File.Exists(path))
            File.Delete(path);
        if (dt == DataType.RUN) {
            rData = null;
        }else {
            pData = null;
        }
    }
}