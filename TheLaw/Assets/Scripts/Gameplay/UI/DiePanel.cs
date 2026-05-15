using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DiePanel : PanelBase
{
    protected override void ButtonOnClick(string buttonName)
    {
        UIManager.Instance.RemovePanel<BattlePanel>();
        switch (buttonName)
        {
            case "Button_Replay":
                ProgressManager.Instance.intoNewLevel(ProgressManager.Instance.level);
                UIManager.Instance.ChangePanel<DiePanel, BattlePanel>();
                break;
            case "Button_ToStartPanel":
                UIManager.Instance.ChangePanel<DiePanel, StartMenuPanel>();
                break;
            default:
                return;
        }
    }
}
