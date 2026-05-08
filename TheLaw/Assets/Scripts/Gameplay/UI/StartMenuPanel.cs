using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartMenuPanel : PanelBase
{

    protected override void ButtonOnClick(string buttonName)
    {
        switch (buttonName)
        {
            case "Button_StartGame":
                if (JsonManager.Instance.LoadDataByType(E_SaveDataType.StoryProgress) == 0)
                {
                    // 没看初始剧情的看初始剧情
                    StoryManager.Instance.LoadStorySegmentByIndex(1);
                    UIManager.Instance.ChangePanel<StartMenuPanel, StoryPanel>();
                }
                else
                {
                    UIManager.Instance.ChangePanel<StartMenuPanel, LevelSelectPanel>();
                }
                break;
            case "Button_QuitGame":
                // 编辑器下停止运行
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // 打包后退出程序
                Application.Quit();
#endif
                break;
            default:
                return;
        }
    }
}
