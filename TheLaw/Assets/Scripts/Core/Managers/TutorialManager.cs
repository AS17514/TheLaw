using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : ManagerBase<TutorialManager>
{
    // 私有无参构造函数，保证单例严谨性
    private TutorialManager() { }

    protected override void Init()
    {
        // 监听选项更新事件
        EventCenter.Instance.AddEventListener(E_EventType.UI_Update_Events, CheckTutorialsOnOptionsUpdated);
    }

    private void CheckTutorialsOnOptionsUpdated(object info = null)
    {
        // 根据当前关卡获取选项池
        E_OptionType optionType = E_OptionType.Level1_Option;
        switch (ProgressManager.Instance.level)
        {
            case 1: optionType = E_OptionType.Level1_Option; break;
            case 2: optionType = E_OptionType.Level2_Option; break;
            case 3: optionType = E_OptionType.Level3_Option; break;
            case 4: optionType = E_OptionType.Level4_Option; break;
            case 5: optionType = E_OptionType.Level5_Option; break;
        }

        OptionBase[] currentOptions = EventManager.Instance.GetOptionPoolByType(optionType);
        if (currentOptions == null) return;

        foreach (var option in currentOptions)
        {
            if (option == null) continue;

            // 原有触发条件不变，仅替换存储检查逻辑
            if (option.BindTutorial != E_TutorialType.OptionNull && option.IsVisible == true)
            {
                CheckAndTriggerTutorial(option.BindTutorial);
            }
        }
    }

    public void CheckAndTriggerTutorial(E_TutorialType tutorialType)
    {
        // 调用JsonManager检查触发状态
        if (!JsonManager.Instance.IsTutorialTriggered(tutorialType))
        {
            // 标记教程为已触发
            JsonManager.Instance.SetTutorialTriggered(tutorialType);

            // 触发教程弹窗
            // UIManager.Instance.CreatPanel<TutorialPanel>(E_UILayer.Top);
            if (UIManager.Instance.GetPanel<TutorialPanel>() == null)
            {
                UIManager.Instance.CreatPanel<TutorialPanel>(E_UILayer.Top);
            }
            UIManager.Instance.GetPanel<TutorialPanel>().ShowTutorialByNaming(tutorialType,
                UIManager.Instance.GetPanel<BattlePanel>());
            Debug.Log($"[Tutorial] 触发新手引导: {tutorialType}");
        }
    }

    /// <summary>
    /// 重置所有教程记录
    /// </summary>
    public void ResetAllTutorials()
    {
        JsonManager.Instance.ResetAllTutorialData();
    }
    
}