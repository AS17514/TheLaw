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
                UIManager.Instance.CreatPanel<BattlePanel>(E_UILayer.Middle);
                break;
            case "Button_ToStartPanel":
                UIManager.Instance.CreatPanel<StartMenuPanel>(E_UILayer.Middle);
                break;
            default:
                return;
        }
    }
}
