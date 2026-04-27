using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : ManagerBase<StateManager>
{
    private Dictionary<System.Enum, Action[]> stateActions = new Dictionary<System.Enum, Action[]>();
    //System.Enum放E_StateType
    private System.Enum currentState;
    private int currentActionIndex = 0;   //当前的状态。
    private Action currentExecutableMethod;//当前状态内行为执行到第几个了
    public void ChangeState(System.Enum newState)
    {
        // 确保字典里有这个状态，并且新状态和当前状态不一样
        if (stateActions.ContainsKey(newState) && (currentState == null || !currentState.Equals(newState)))
        {
            currentState = newState;
            currentActionIndex = 0; // 重置行为队列
            currentExecutableMethod = stateActions[currentState][currentActionIndex];
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityState,currentState);
        }
    }
    /// <summary>
    /// 外部调用，用来执行怪物的行为。
    /// </summary>
    public void ExecuteCurrentAction()
    {
        if (currentExecutableMethod != null)
        {
            currentExecutableMethod.Invoke(); 
            currentActionIndex++;             
            
            // 行为轮流循环
            if (currentActionIndex >= stateActions[currentState].Length)
            {
                currentActionIndex = 0; 
            }
            
            currentExecutableMethod = stateActions[currentState][currentActionIndex];
        }
    }
    /// <summary>
    /// 关卡切换/怪物死亡时，清空一下字典，防止上一关的数据残留
    /// 感觉用不到，但是姑且写一下。
    /// </summary>
    public void ClearStates()
    {
        stateActions.Clear();
        currentState = null;
        currentExecutableMethod = null;
    }

}
