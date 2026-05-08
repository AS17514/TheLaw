using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using Newtonsoft.Json;
using UnityEngine;

public enum E_SaveDataType
{
    LevelProgress,
    StoryProgress
}

public class SaveData
{
    public int storyProgress;
    public int levelProgress;
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
    /// <summary>
    /// 创建默认存档文件
    /// </summary>
    void CreatDefaultSave()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("不存在存档文件，创建默认初始存档");
            AdjustSaveDataByType(E_SaveDataType.LevelProgress, 1);
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
    /// <summary>
    /// 根据参数调整存档内容，文件不存在则设定值，其余为默认值
    /// </summary>
    /// <param name="type">需要调整的内容</param>
    /// <param name="value">调整值</param>
    public void AdjustSaveDataByType(E_SaveDataType type, int value)
    {
        SaveData saveData = Load(savePath);
        switch (type)
        {
            case E_SaveDataType.LevelProgress:
                saveData.levelProgress = value;
                break;
            case E_SaveDataType.StoryProgress:
                saveData.storyProgress = value;
                break;
            default:
                break;
        }
        Save(savePath, saveData);
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
    /// <summary>
    /// 从存档文件读取当前关卡最新进度，如有错误返回-1
    /// </summary>
    /// <returns>当前最新进度</returns>
    public int LoadDataByType(E_SaveDataType type)
    {
        SaveData saveData = Load(savePath);
        if (saveData == null)
        {
            Debug.LogWarning($"{savePath}未找到存档文件，返回-1");
            CreatDefaultSave();
            return -1;
        }
        else
        {
            switch (type)
            {
                case E_SaveDataType.LevelProgress:
                    return saveData.levelProgress;
                case E_SaveDataType.StoryProgress:
                    return saveData.storyProgress;
                default:
                    return -1;
            }
        }
    }
}
