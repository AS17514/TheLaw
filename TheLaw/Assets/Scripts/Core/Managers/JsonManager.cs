using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public enum E_Save
{
    Save1,
    Save2,
    Save3
}
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
    /// <summary>
    /// 保存对象到指定json文件
    /// </summary>
    /// <param name="data">需要保存的对象</param>
    /// <param name="fileName">存档枚举</param>
    /// <typeparam name="T">需要保存的对象类型</typeparam>
    public void SaveData<T>(T data, E_Save fileName)
    {
        string savePath = Path.Combine(path, fileName.ToString(), ".json");
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(savePath, jsonData);
        EventCenter.Instance.EventTrigger(E_EventType.SaveData);
        Debug.Log($"已将数据存储到{savePath}");
    }
    /// <summary>
    /// 从指定json文件读取对象
    /// </summary>
    /// <param name="fileName">存档枚举</param>
    /// <typeparam name="T">需要读取的对象类型</typeparam>
    /// <returns></returns>
    public object LoadData<T>(E_Save fileName)
    {
        string loadPath = Path.Combine(path, fileName.ToString(), ".json");
        if (!File.Exists(loadPath))
        {
            Debug.LogError($"未读取到{loadPath}存档文件");
            return null;
        }
        string jsonData = File.ReadAllText(loadPath);
        EventCenter.Instance.EventTrigger(E_EventType.LoadData);
        Debug.Log($"已将数据从{loadPath}读取");
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}
