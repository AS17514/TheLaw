using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


public class JsonManager : ManagerBase<JsonManager>
{
    string path;
    JsonManager()
    {
        // 初始化存储路径以及文件夹
        path = Path.Combine(Application.persistentDataPath, "TheLaw/Data");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    public void SaveData<T>(T data, string fileName)
    {
        string savePath = Path.Combine(path, fileName, ".json");
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(savePath, jsonData);
        EventCenter.Instance.EventTrigger(E_EventType.SaveData);
    }
}
