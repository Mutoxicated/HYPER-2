using UnityEngine;

public class RunData : IData
{
    public DataType Type() {
        return DataType.RUN;
    }

    public string Path() {
        return Application.dataPath + System.IO.Path.AltDirectorySeparatorChar + "RunData.json";
    }
}
