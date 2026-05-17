using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuPanel : PanelBase
{
    public override E_BGM? BGMType => E_BGM.StartMenu;

    protected override void ButtonOnClick(string buttonName)
    {
        switch (buttonName)
        {
            case "Button_StartGame":
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.StartGameClick, false });
                GetControl<Button>("Button_StartGame").interactable = false;
                if (JsonManager.Instance.LoadDataByType(E_SaveDataType.StoryProgress) == 0)
                {
                    // 没看初始剧情的看初始剧情
                    StoryManager.Instance.LoadStorySegmentByIndex(1);
                    GetComponentInChildren<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
                    {
                        UIManager.Instance.ChangePanel<StartMenuPanel, StoryPanel>();
                    });
                }
                else
                {
                    GetComponentInChildren<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
                    {
                        UIManager.Instance.ChangePanel<StartMenuPanel, LevelSelectPanel>();
                    });
                }

                break;
            case "Button_QuitGame":
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                    new object[] { E_SFX.QuitGameClick, false });
                DOVirtual.DelayedCall(2f, null).OnComplete(() =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                });
                break;
            case "Button_Settings":
                UIManager.Instance.CreatPanel<SettingsPanel>(E_UILayer.Top);
                break;
            default:
                break;
        }
    }
}
