using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : ManagerBase<TutorialManager>
{
    // 建议：加一个私有无参构造函数。
    // 因为你的 ManagerBase 使用了 Activator.CreateInstance(typeof(T), true) 允许调用私有构造。
    // 加上私有构造可以防止外部不小心 new TutorialManager()，保证单例的严谨性。
    private TutorialManager() { }
    
    protected override void Init()
    {
        // 父类在第一次创建实例时，会自动调用这个方法
        // 监听现有的选项更新事件
        EventCenter.Instance.AddEventListener(E_EventType.UI_Update_Events, CheckTutorialsOnOptionsUpdated);
    }

    private void CheckTutorialsOnOptionsUpdated(object info = null)
    {
        // 根据当前关卡等级获取对应的选项池
        E_OptionType optionType = E_OptionType.Level1_Option;
        switch (ProgressManager.Instance.level)
        {
            case 1:
                optionType = E_OptionType.Level1_Option;
                break;
            case 2:
                optionType = E_OptionType.Level2_Option;
                break;
            case 3:
                optionType = E_OptionType.Level3_Option;
                break;
            case 4:
                optionType = E_OptionType.Level4_Option;
                break;
            case 5:
                optionType = E_OptionType.Level5_Option;
                break;
        }

        OptionBase[] currentOptions = EventManager.Instance.GetOptionPoolByType(optionType);

        // 加上判空保护，防止获取失败报错
        if (currentOptions == null) return;

        foreach (var option in currentOptions)
        {
            // 判空，防止数组中有空洞（比如索引为0的地方是null）
            if (option == null) continue;

            // 如果这个选项带有新手引导，并且它当前计算出来的状态是可见的
            if (option.BindTutorial != E_TutorialType.None && option.IsVisible == true)
            {
                // 检测是否是第一次，并触发
                CheckAndTriggerTutorial(option.BindTutorial);
            }
        }
    }

    private void CheckAndTriggerTutorial(E_TutorialType tutorialType)
    {
        string saveKey = "Tutorial_" + tutorialType.ToString();
        
        if (PlayerPrefs.GetInt(saveKey, 0) == 0)
        {
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();

            // 通知前端弹窗
            EventCenter.Instance.EventTrigger(E_EventType.UI_ShowTutorial, tutorialType);
            Debug.Log($"[Tutorial] 触发新手引导: {tutorialType}");
        }
    }
    /// <summary>
    /// 重置所有新手引导记录
    /// </summary>
    public void ResetAllTutorials()
    {
        // 获取 E_TutorialType 枚举中定义的所有值
        var allTutorials = System.Enum.GetValues(typeof(E_TutorialType));

        foreach (E_TutorialType type in allTutorials)
        {
            if (type == E_TutorialType.None) continue;

            string saveKey = "Tutorial_" + type.ToString();
        
            // 检查是否存在该键，存在则删除
            if (PlayerPrefs.HasKey(saveKey))
            {
                PlayerPrefs.DeleteKey(saveKey);
            }
        }

        // 强制保存修改到磁盘
        PlayerPrefs.Save();
    
        Debug.Log("<color=yellow>[Tutorial] 所有新手引导记录已针对性重置</color>");
    }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class TutorialManager 
// {
//     public static TutorialManager Instance = new TutorialManager();
//
//     public void Init()
//     {
//         // 监听你现有的选项更新事件！这是最完美的切入点
//         EventCenter.Instance.AddEventListener(E_EventType.UI_Update_Events, CheckTutorialsOnOptionsUpdated);
//     }
//
//     private void CheckTutorialsOnOptionsUpdated(object info = null)
//     {
//         // 假设这里可以通过 EventManager 拿到当前关卡的所有选项
//         // 根据你的代码，可能是类似 EventManager.Instance.optionPool[当前关卡Type] 的集合
//         E_OptionType optionType=E_OptionType.Level1_Option;
//         switch (ProgressManager.Instance.level)
//         {
//             case 1:
//                 optionType=E_OptionType.Level1_Option;
//                 break;
//             case 2:
//                 optionType=E_OptionType.Level2_Option;
//                 break;
//             case 3:
//                 optionType=E_OptionType.Level3_Option;
//                 break;
//             case 4:
//                 optionType=E_OptionType.Level4_Option;
//                 break;
//             case 5:
//                 optionType=E_OptionType.Level5_Option;
//                 break;
//             
//         }
//         OptionBase[] currentOptions = EventManager.Instance.GetOptionPoolByType(optionType); 
//
//         foreach (var option in currentOptions)
//         {
//             // 如果这个选项带有新手引导，并且它当前计算出来的状态是可见的
//             if (option.BindTutorial != E_TutorialType.None && option.IsVisible == true)
//             {
//                 // 检测是否是第一次，并触发
//                 CheckAndTriggerTutorial(option.BindTutorial);
//             }
//         }
//     }
//
//     private void CheckAndTriggerTutorial(E_TutorialType tutorialType)
//     {
//         string saveKey = "Tutorial_" + tutorialType.ToString();
//         
//         if (PlayerPrefs.GetInt(saveKey, 0) == 0)
//         {
//             PlayerPrefs.SetInt(saveKey, 1);
//             PlayerPrefs.Save();
//
//             // 通知前端弹窗
//             EventCenter.Instance.EventTrigger(E_EventType.UI_ShowTutorial, tutorialType);
//             Debug.Log($"[Tutorial] 触发新手引导: {tutorialType}");
//         }
//     }
// }
