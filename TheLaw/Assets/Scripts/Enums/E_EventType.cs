using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 孩子们，我是给事件中心用的。
/// </summary>
public enum E_EventType
{
    #region UI更新
    #region 战斗界面
    // 骰子
    UI_Update_SelectedDice,
    UI_Update_TimeDice1Count,
    UI_Update_TimeDice1SelectedCount,
    UI_Update_TimeDice2Count,
    UI_Update_TimeDice2SelectedCount,
    UI_Update_TimeDice3Count,
    UI_Update_TimeDice3SelectedCount,
    UI_Update_TimeDice4Count,
    UI_Update_TimeDice4SelectedCount,
    UI_Update_WildDiceCount,
    UI_Update_ActionDice,
    UI_Update_MindDice,
    UI_Update_IsConditionNotMet,
    // 玩家
    UI_Update_PlayerHP,
    UI_Update_PlayerBuff,
    // 怪
    UI_Update_EntityHP,
    UI_Update_EntityBuff,
    UI_Update_EntityPartHP,
    UI_Update_EntityPartBreakState,
    UI_Update_EntityState,
    UI_Update_EntityIntent,
    // 时间
    UI_Update_Phase,
    UI_Update_TimeProgress,
    UI_Update_MaxTimeProgress,
    UI_Update_TimeDicePerPhase,
    // 事件
    UI_Update_Events,
    #endregion
    #endregion
    #region 存档
    SaveData,
    LoadData,
    #endregion


}

