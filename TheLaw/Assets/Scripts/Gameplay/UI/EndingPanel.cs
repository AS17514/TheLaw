using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingPanel : PanelBase
{
    float _delay = 0.1f;

    void Update()
    {
        if (_delay > 0) { _delay -= Time.deltaTime; return; }
        if (Input.anyKeyDown)
            UIManager.Instance.ChangePanel<EndingPanel, StartMenuPanel>();
    }
}
