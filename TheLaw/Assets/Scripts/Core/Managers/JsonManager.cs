using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using Newtonsoft.Json;
using UnityEngine;

public class SaveData
{
    public SaveData(int level)
    {
        this.level = level;
    }
    public int level;
}
public class JsonManager : ManagerBase<JsonManager>
{
    string path;
    string savePath;
    JsonManager()
    {
        // 初始化存储路径以及文件夹
        path = Application.persistentDataPath + "/Data/";
        savePath = path + "SaveData.json";
        if (!Directory.Exists(path))
        {
            Debug.LogWarning($"没文件夹，创建文件夹{path}");
            Directory.CreateDirectory(path);
        }
        // 没有存档文件，默认创建初始存档
        CreatDefaultSave();
    }
    void CreatDefaultSave()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("不存在存档文件，创建默认初始存档");
            SaveLevel(1);
        }
    }
    /// <summary>
    /// 保存对象到指定json文件
    /// </summary>
    /// <param name="data">需要保存的对象</param>
    public void Save(string path, SaveData data)
    {
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, jsonData);
        Debug.Log($"已将数据存储到{path}");
    }
    public void SaveLevel(int level)
    {
        Save(savePath, new SaveData(level));
    }
    /// <summary>
    /// 从json文件读取对象
    /// </summary>
    /// <returns></returns>
    public SaveData Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"未读取到{path}存档文件");
            return null;
        }
        string jsonData = File.ReadAllText(path);
        Debug.Log($"已将数据从{path}读取");
        return JsonConvert.DeserializeObject<SaveData>(jsonData);
    }
    public int LoadLevel()
    {
        SaveData saveData = Load(savePath);
        if (saveData == null)
        {
            Debug.LogWarning($"{savePath}未找到存档文件，返回关卡为1");
            CreatDefaultSave();
            return 1;
        }
        else
        {
            return saveData.level;
        }
    }
}
