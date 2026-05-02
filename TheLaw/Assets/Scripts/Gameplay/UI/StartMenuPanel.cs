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
                ProgressManager.Instance.intoNewLevel(1);
                UIManager.Instance.ChangePanel<StartMenuPanel, BattlePanel>();
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
