using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 改成了抽象类，防止某个地方创建了这个脚本，导致它生命周期当中自动挂载的效果生效。 所有的怪物都会继承这个抽象类。
/// </summary>
public abstract class Entity : CharacterBase
{
    #region 用于计算buff管理器内部的逻辑

    protected Dictionary<E_BuffType, int> buffs = new Dictionary<E_BuffType, int>
    {
        {E_BuffType.Desire,0 },
        {E_BuffType.Tatters,0}
    };
    public Dictionary<E_BuffType, int> UI_buffs { get { return buffs; } }//给UI初始化用的获取被保护字典的属性
    public override bool IsPlayer => false;
    public void AddBuff(E_BuffType type, int amount)
    {
        if (!buffs.ContainsKey(type)) buffs[type] = 0;

        buffs[type] += amount;

        // 数值一变，立刻通过事件中心广播出去
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityBuff, buffs);
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
    public List<Part> parts = new List<Part>();
    public string entityName;
    public override void Die()
    {
        //怪物死亡，玩家胜利
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP, hp);
    }

    public virtual void InitEntity(int initialDesire = 0, int maxHp = 10)
    {
        AddBuff(E_BuffType.Desire, initialDesire);
        this.maxHp = maxHp;
        this.hp = maxHp;
    }
    public virtual void ManualInit()
    {
        // 游戏一开始，就把自己交到管理器手里
        BuffManager.Instance.Register(this);
    }
    private void OnDestroy()
    {
        // 死亡时，主动告诉管理器删除
        if (BuffManager.Instance != null)
        {
            BuffManager.Instance.Unregister(this);
        }
    }
}
