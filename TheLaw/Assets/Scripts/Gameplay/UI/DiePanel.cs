using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DiePanel : PanelBase
{
    void Start()
    {
        EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_BGM,
            new object[] { E_BGM.GameOver, true });
    }

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
