using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ActionNode
{
    public E_IntentType Intent; // 给 UI 看的数据
    public Action ExecuteLogic; // 给 StateManager 执行的逻辑

    // 构造函数方便快速实例化
    public ActionNode(E_IntentType intent, Action logic)
    {
        Intent = intent;
        ExecuteLogic = logic;
    }

}
