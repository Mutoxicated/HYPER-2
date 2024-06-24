using System.IO;
using UnityEngine;

public enum DataType {
    RUN,
    PLAYER
}

public interface IData {
    DataType Type(); 
    string Path();
}

public class RunDataSave : MonoBehaviour
{
    public static PlayerData pData;
    public static RunData rData;

    private void Awake()
    {
        if (File.Exists(new RunData().Path()))
        {
            UpdateData(DataType.RUN);
        }else {
            CreateData(DataType.RUN);
        }
        
        if (File.Exists(new PlayerData().Path()))
        {
            UpdateData(DataType.PLAYER);
        }else {
            CreateData(DataType.PLAYER);
        }
    }

    private static IData GetData(DataType dt) {
        return dt == DataType.RUN? rData : pData;
    } 

    public static void CreateData(DataType dt)
    {
        if (dt == DataType.RUN)
            rData = new RunData();
        else
            pData = new PlayerData();
        UpdateJsonData(dt);
        UpdateData(dt);
    }

    public static void UpdateJsonData(DataType dt)
    {
        string savePath = GetData(dt).Path();

        //Debug.Log("Saving rData to path: "+ savePath);

        string json = JsonUtility.ToJson(rData);
        //Debug.Log(json);

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
    }

    public static void UpdateData(DataType dt)
    {
        using StreamReader reader = new StreamReader(GetData(dt).Path());
        string json = reader.ReadToEnd();

        rData = JsonUtility.FromJson<RunData>(json);
    }

    public static void RemoveData(DataType dt){
        var path = GetData(dt).Path();
        if (File.Exists(path))
            File.Delete(path);
        if (dt == DataType.RUN) {
            rData = null;
        }else {
            pData = null;
        }
    }
}