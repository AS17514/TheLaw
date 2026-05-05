using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : ManagerBase<StateManager>
{
    private Dictionary<System.Enum, ActionNode[]> stateActions = new Dictionary<System.Enum, ActionNode[]>();
    private Dictionary<System.Enum, DesireNode[]> stateDesires = new Dictionary<System.Enum, DesireNode[]>();
    //System.Enum放E_StateType
    public System.Enum currentState;//当前的状态。
    public Enum UI_currentState { get { return currentState; } }//开放给UI用的，获取当前状态的属性
    private int currentActionIndex = 0;//当前状态内行为执行到第几个了
    private ActionNode currentExecutableAction;
    public E_IntentType UI_currentExecutableAction { get { return currentExecutableAction.Intent; } }//开放给UI用的，获取当前行为的属性
    private int currentDesireIndex = 0;
    private DesireNode currentExecutableDesire;
    public E_DesireType UI_currentExecutableDesire { get { return currentExecutableDesire.Desire; } }//开放给UI用的，获取当前愿望的属性
    public void ChangeState(System.Enum newState)
    {
        // 确保字典里有这个状态，并且新状态和当前状态不一样
        if (stateActions.ContainsKey(newState) && (currentState == null || !currentState.Equals(newState)))
        {
            currentState = newState;
            currentActionIndex = 0; // 重置行为队列
            currentExecutableAction = (stateActions[currentState] != null && stateActions[currentState].Length > 0)
                ? stateActions[currentState][0] : null;

            currentDesireIndex = 0;// 重置愿望队列
            currentExecutableDesire = (stateDesires.ContainsKey(currentState) && stateDesires[currentState] != null
                                                                              && stateDesires[currentState].Length > 0)
                ? stateDesires[currentState][0] : null;

            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityState);
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityWish);
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityAction);
        }

        #region Level2特殊逻辑

        if (ProgressManager.Instance.level == 2)
        {
            if (currentState is E_StateType_2.composed)//坦然状态:时间进度变为1
            {
                ProgressManager.Instance.SetCurrentTimeProgress(1);
            }
            else
            {
                ProgressManager.Instance.SetCurrentTimeProgress(4);
            }
        }

        if (ProgressManager.Instance.level == 2 && currentState is E_StateType_2.normal)
        {
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        }

        if (ProgressManager.Instance.level == 2 && currentState is E_StateType_2.ashamed)
        {
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        }
        #endregion
    }
    /// <summary>
    /// 外部调用，用来执行怪物的行为。（时间进度结束时调用）
    /// </summary>
    public void ExecuteCurrentAction()
    {
        if (currentExecutableAction != null)
        {
            currentExecutableAction.ExecuteLogic?.Invoke();
            currentActionIndex++;

            // 行为轮流循环
            if (currentActionIndex >= stateActions[currentState].Length)
            {
                currentActionIndex = 0;
            }

            currentExecutableAction = stateActions[currentState][currentActionIndex];
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityAction);
        }
    }
    /// <summary>
    /// 不执行怪物的行为，但是把怪物的行为切到下一个，第二关使用
    /// </summary>
    public void ChangeCurrentActionToNextAction()
    {
        currentActionIndex++;

        // 行为轮流循环
        if (currentActionIndex >= stateActions[currentState].Length)
        {
            currentActionIndex = 0;
        }

        currentExecutableAction = stateActions[currentState][currentActionIndex];
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityAction);
    }

    /// <summary>
    /// 执行怪物愿望（时间段结束时调用）
    /// </summary>
    public void ExecuteCurrentDesire()
    {
        if (currentExecutableDesire != null)
        {
            currentExecutableDesire.ExecuteLogic?.Invoke();
            currentDesireIndex++;

            if (currentDesireIndex >= stateDesires[currentState].Length)
            {
                currentDesireIndex = 0;
            }
            currentExecutableDesire = stateDesires[currentState][currentDesireIndex];
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityWish);
        }
    }
    /// <summary>
    /// 关卡切换/怪物死亡时，清空一下字典，防止上一关的数据残留
    /// 感觉用不到，但是姑且写一下。
    /// </summary>
    public void ClearStates()
    {
        stateActions.Clear();
        stateDesires.Clear();
        currentState = null;
        currentExecutableAction = null;
        currentExecutableDesire = null;
    }
    /// <summary>
    /// 怪物在 Start/OnEnable 时调用这个方法，把自己的状态、行为和愿望注册进来
    /// </summary>
    /// <param name="state"></param>
    /// <param name="actions">行动</param>
    /// <param name="desires">欲望</param>
    public void RegisterStateData(System.Enum state, ActionNode[] actions, DesireNode[] desires)
    {
        stateActions[state] = actions;
        stateDesires[state] = desires;
    }
}
