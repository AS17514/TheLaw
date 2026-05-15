using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : PanelBase
{
    int nowLevel;
    int selectedLevel;
    public override E_BGM? BGMType => E_BGM.StartMenu;
    Sprite unknown;
    protected override void Awake()
    {
        base.Awake();
        InitEvent();
        nowLevel = JsonManager.Instance.LoadDataByType(E_SaveDataType.LevelProgress);
        unknown = Resources.Load<Sprite>("Prefabs/UI/SelectLevel/Image_Unknown");
        print(nowLevel);
        // 默认选中当前最新进度
        selectedLevel = nowLevel;
        if (nowLevel == 6)
        {
            selectedLevel = 5;
        }

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
            if (nowLevel < i)
            {
                button.interactable = false;
                Image img = button.GetComponent<Image>();
                Color c = img.color;
                c.a = 0.3f;
                img.color = c;

                Transform imgTrans = button.transform.Find("Image");
                if (imgTrans != null)
                {
                    Image childImg = imgTrans.GetComponent<Image>();
                    {
                        childImg.sprite = unknown;
                        childImg.SetNativeSize();
                    }
                }
            }
        }
    }
    void UpdateSelectedLevelTitle(object obj)
    {
        TextMeshProUGUI tmp = GetControl<TextMeshProUGUI>("Text (TMP)_LevelTitle");
        switch (selectedLevel)
        {
            case 0:
                tmp.text = "开端";
                break;
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
                tmp.text = "住";
                break;
            case 5:
                tmp.text = "天平";
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
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.EnterLevelClick, false });
                if (selectedLevel != 0)
                {
                    Debug.Log($"进入第{selectedLevel}关开头剧情");
                    StoryManager.Instance.LoadStorySegmentByIndex(2 * selectedLevel);
                }
                else
                {
                    Debug.Log($"进入初始剧情");
                    StoryManager.Instance.LoadStorySegmentByIndex(1);
                }
                UIManager.Instance.ChangePanel<LevelSelectPanel, StoryPanel>();
                break;
            case "Button_Back":
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.BackClick, false });
                UIManager.Instance.ChangePanel<LevelSelectPanel, StartMenuPanel>();
                break;
            case "Button_Level0":
                selectedLevel = 0;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.LevelSelectClick, false });
                break;
            case "Button_Level1":
                selectedLevel = 1;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.LevelSelectClick, false });
                break;
            case "Button_Level2":
                selectedLevel = 2;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.LevelSelectClick, false });
                break;
            case "Button_Level3":
                selectedLevel = 3;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.LevelSelectClick, false });
                break;
            case "Button_Level4":
                selectedLevel = 4;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.LevelSelectClick, false });
                break;
            case "Button_Level5":
                selectedLevel = 5;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedLevelTitle);
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.LevelSelectClick, false });
                break;
            default:
                break;
        }
    }
}
