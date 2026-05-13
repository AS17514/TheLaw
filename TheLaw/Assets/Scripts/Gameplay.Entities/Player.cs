using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{

    #region 用于计算buff管理器内部的逻辑
    private Dictionary<E_BuffType, int> buffs = new Dictionary<E_BuffType, int>
    {
        {E_BuffType.Desire,0 },
        {E_BuffType.Tatters,0},
        {E_BuffType.Up,0},
        {E_BuffType.Down,0},
        {E_BuffType.Left,0},
        {E_BuffType.Right,0},
    };
    public Dictionary<E_BuffType, int> UI_buffs
    {
        get { return buffs; }
    }
    public override bool IsPlayer => true;
    public void AddBuff(E_BuffType type, int amount)
    {
        if (!buffs.ContainsKey(type)) buffs[type] = 0;
        buffs[type] += amount;


        // 数值一变，立刻通过事件中心广播出去
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerBuff);
    }

    public void RemoveLevel5Buff()
    {
        buffs[E_BuffType.Left] = 0;
        buffs[E_BuffType.Right] = 0;
        buffs[E_BuffType.Up] = 0;
        buffs[E_BuffType.Down] = 0;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerBuff);
    }
    /// <summary>
    /// 提供给管理器的查询buff方法
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetBuff(E_BuffType type)
    {
        return buffs.ContainsKey(type) ? buffs[type] : 0;
    }
    #endregion
    public Dictionary<E_WishType, bool> wishUnlockProgress = new Dictionary<E_WishType, bool>();

    private void OnDestroy()
    {
        if (BuffManager.Instance != null)
        {
            BuffManager.Instance.Unregister(this);
        }
    }
    public override void Die()
    {
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerDied);
    }

    public override void TakeDamage(int damage)
    {
        int i = 0;
        if (buffs[E_BuffType.Left] > 0)
            i = 1;
        base.TakeDamage(damage+i);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerHP);
        // 致死时 Die() 已由 CharacterBase.TakeDamage 调用，勿重复触发 UI_Update_PlayerDied
    }

    /// <summary>
    /// 执行固有行动
    /// </summary>
    /// <param name="inherentActionType"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void ExecuteInherentAction(E_InherentActionType inherentActionType)
    {

    }
    /// <summary>
    /// 执行许愿
    /// </summary>
    /// <param name="wishType"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void ExecuteWish(E_WishType wishType)
    {

    }
    /// <summary>
    /// 执行律
    /// </summary>
    /// <param name="lwaType"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void ExecuteLaw(E_LawType lwaType)
    {

    }
    /// <summary>
    /// 感觉每进入下一关得调用一下，把生命回满到10点，同时设置初始的欲望。比如第三关，玩家初始就有欲望。
    /// </summary>
    /// <param name="maxhp"></param>
    /// <param name="hp"></param>
    /// <param name="initialDesire"></param>
    public void InitPlayer(int initialDesire = 0, int maxHp = 10)
    {

        BuffManager.Instance.Register(this);

        //清空上一关的buff
        List<E_BuffType> keys = new List<E_BuffType>(buffs.Keys);
        foreach (var key in keys)
        {
            buffs[key] = 0;
        }

        // 赋予初始欲望（AddBuff 内部会自动触发 UI_Update_PlayerBuff 广播，这样所有归零的 Buff 也会同步刷新给 UI）
        AddBuff(E_BuffType.Desire, initialDesire);

        this.maxHp = maxHp;
        this.hp = maxHp;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerHP);
    }
}
