using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 孩子们，我是给事件中心用的。
/// </summary>
public enum E_EventType
{
    #region Audio
    #region 音频播放
    Audio_Play_BGM,
    Audio_Play_SFX,
    #endregion
    #region 音频更新
    Audio_Update_BGMVolume,
    Audio_Update_SFXVolume,
    #endregion
    #endregion

    #region UI更新
    #region 选关界面
    UI_Update_SelectedLevelTitle,
    #endregion
    #region 战斗界面
    #region  骰子
    UI_Update_SelectedDice,
    UI_Update_TimeDiceCount,
    UI_Update_TimeDiceSelectedCount,
    UI_Update_TimeDice,
    UI_Update_WildDiceCount,
    UI_Update_ActionDice,
    UI_Update_MindDice,
    UI_Update_IsConditionNotMet,
    UI_Update_EntityDice,
    #endregion
    #region 玩家
    UI_Update_PlayerHP,
    UI_Update_PlayerBuff,
    UI_Update_PlayerDied,
    #endregion
    #region 怪
    UI_Update_EntityHP,
    UI_Update_EntityBuff,
    UI_Update_EntityPart,
    UI_Update_EntityState,
    UI_Update_EntityAction,
    UI_Update_EntityWish,
    UI_Update_EntityDied,
    #endregion
    #region 时间
    UI_Update_Phase,
    UI_Update_TimeProgress,
    UI_Update_MaxTimeProgress,
    UI_Update_TimeDicePerPhase,
    #endregion
    #region 事件
    UI_Update_Events,
    UI_Update_WishToAvailable,
    UI_Update_WishToUnavailable,
    #endregion
    #endregion
    #endregion

    #region 后端
    Logic_PlayerActionExecuted
    #endregion


}

