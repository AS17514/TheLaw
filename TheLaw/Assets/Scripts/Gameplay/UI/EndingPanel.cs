using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingPanel : PanelBase
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            UIManager.Instance.ChangePanel<EndingPanel, StartMenuPanel>();
        }
    }
}
