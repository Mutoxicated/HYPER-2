using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : IData
{
    public string Path()
    {
        return Application.dataPath + System.IO.Path.AltDirectorySeparatorChar + "PlayerData.json";
    }

    public DataType Type()
    {
        return DataType.PLAYER;
    }
}
