using System.Collections;
using System.Collections.Generic;
using System.IO;
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

public class TutorialSaveData
{
    public Dictionary<string, int> TutorialTriggerStates = new Dictionary<string, int>();
}

public class AudioSettingsData
{
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.5f;
}

public class JsonManager : ManagerBase<JsonManager>
{
    string path;
    string savePath;
    string tutorialSavePath;
    string audioSettingsPath;

    JsonManager()
    {
        // 初始化存储路径
        path = Application.persistentDataPath + "/Data/";
        savePath = path + "SaveData.json";
        tutorialSavePath = path + "TutorialSaveData.json";
        audioSettingsPath = path + "AudioSettings.json";

        // 创建数据文件夹
        if (!Directory.Exists(path))
        {
            Debug.LogWarning($"没文件夹，创建文件夹{path}");
            Directory.CreateDirectory(path);
        }
        // 初始化默认存档
        CreatDefaultSave();
        // 初始化默认教程存档
        CreateDefaultTutorialSave();
        // 初始化默认音频存档
        CreateDefaultAudioSettings();
    }
    void CreateDefaultAudioSettings()
    {
        if (!File.Exists(audioSettingsPath))
            Save(audioSettingsPath, new AudioSettingsData());
    }
    void CreatDefaultSave()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("不存在存档文件，创建默认初始存档");
            AdjustSaveDataByType(E_SaveDataType.LevelProgress, 1);
        }
    }
    void CreateDefaultTutorialSave()
    {
        if (!File.Exists(tutorialSavePath))
        {
            Debug.LogWarning("不存在教程存档文件，创建默认教程存档");
            Save(tutorialSavePath, new TutorialSaveData());
        }
    }
    public void Save<T>(string path, T data)
    {
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, jsonData);
        Debug.Log($"已将{typeof(T).Name}数据存储到{path}");
    }
    public T Load<T>(string path) where T : new()
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"未读取到{path}文件，返回默认实例");
            return new T();
        }
        string jsonData = File.ReadAllText(path);
        T data = JsonConvert.DeserializeObject<T>(jsonData);
        Debug.Log($"已将{typeof(T).Name}数据从{path}读取");
        return data ?? new T();
    }
    public void AdjustSaveDataByType(E_SaveDataType type, int value)
    {
        SaveData saveData = Load<SaveData>(savePath);
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
    public int LoadDataByType(E_SaveDataType type)
    {
        SaveData saveData = Load<SaveData>(savePath);
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
    public bool IsTutorialTriggered(E_TutorialType tutorialType)
    {
        if (tutorialType == E_TutorialType.OptionNull) return true;

        string key = tutorialType.ToString();
        TutorialSaveData tutorialData = Load<TutorialSaveData>(tutorialSavePath);

        // 存在键且值为1 = 已触发
        if (tutorialData.TutorialTriggerStates.TryGetValue(key, out int state))
        {
            return state == 1;
        }
        // 无记录 = 未触发
        return false;
    }
    public void SetTutorialTriggered(E_TutorialType tutorialType)
    {
        if (tutorialType == E_TutorialType.OptionNull) return;

        string key = tutorialType.ToString();
        TutorialSaveData tutorialData = Load<TutorialSaveData>(tutorialSavePath);

        // 更新触发状态为1
        tutorialData.TutorialTriggerStates[key] = 1;
        Save(tutorialSavePath, tutorialData);

        Debug.Log($"[Tutorial] 标记教程{tutorialType}为已触发（JSON存储）");
    }
    public void ResetAllTutorialData()
    {
        TutorialSaveData tutorialData = new TutorialSaveData();
        tutorialData.TutorialTriggerStates.Clear(); // 清空所有教程记录
        Save(tutorialSavePath, tutorialData);

        Debug.Log("<color=yellow>[Tutorial] 所有新手引导记录已通过JSON重置</color>");
    }
    public void SaveAudioSettings(float bgmVolume, float sfxVolume)
    {
        AudioSettingsData data = Load<AudioSettingsData>(audioSettingsPath);
        data.bgmVolume = bgmVolume;
        data.sfxVolume = sfxVolume;
        Save(audioSettingsPath, data);
    }
    public AudioSettingsData LoadAudioSettings()
    {
        return Load<AudioSettingsData>(audioSettingsPath);
    }
}