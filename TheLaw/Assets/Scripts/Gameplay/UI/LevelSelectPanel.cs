using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : PanelBase
{
    int nowLevel;
    int selectedLevel;
    protected override void Awake()
    {
        base.Awake();
        InitEvent();
        nowLevel = JsonManager.Instance.LoadLevel();
        print(nowLevel);
        // 默认选中当前最新进度
        selectedLevel = nowLevel;

        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
        InitLevelButton();
    }
    void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.UI_Update_SelectedLevelTitle, UpdateSelectedLevelTitle);
    }
    void InitLevelButton()
    {
        for (int i = 1; i < 6; i++)
        {
            Button button = GetControl<Button>($"Button_Level{i}");
            if (nowLevel >= i)
            {
                button.interactable = true;
            }
            else
            {
                break;
            }
        }
    }
    void UpdateSelectedLevelTitle(object obj)
    {
        TextMeshProUGUI tmp = GetControl<TextMeshProUGUI>("Text (TMP)_LevelTitle");
        switch (selectedLevel)
        {
            case 1:
                tmp.text = "食";
                break;
            case 2:
                tmp.text = "衣";
                break;
            case 3:
                tmp.text = "行";
                break;
            case 4:
                tmp.text = "???";
                break;
            case 5:
                tmp.text = "???";
                break;
            default:
                break;
        }
    }
    void InitEvent()
    {
        EventCenter.Instance.AddEventListener(E_EventType.UI_Update_SelectedLevelTitle, UpdateSelectedLevelTitle);
    }
    protected override void ButtonOnClick(string buttonName)
    {
        switch (buttonName)
        {
            case "Button_EnterLevel":
                Debug.Log($"进入第{selectedLevel}关");
                ProgressManager.Instance.intoNewLevel(selectedLevel);
                UIManager.Instance.ChangePanel<LevelSelectPanel, BattlePanel>();
                break;
            case "Button_Back":
                UIManager.Instance.ChangePanel<LevelSelectPanel, StartMenuPanel>();
                break;
            case "Button_Level1":
                selectedLevel = 1;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                break;
            case "Button_Level2":
                selectedLevel = 2;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                break;
            case "Button_Level3":
                selectedLevel = 3;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                break;
            case "Button_Level4":
                selectedLevel = 4;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                break;
            case "Button_Level5":
                selectedLevel = 5;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                break;
            default:
                break;
        }
    }
}
