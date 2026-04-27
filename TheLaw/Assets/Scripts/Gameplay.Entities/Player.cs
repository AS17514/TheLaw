using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    public  Dictionary<E_WishType,bool> wishUnlockProgress=new Dictionary<E_WishType,bool>();
    public int initialDesire;
    public override void Die()
    {
        throw new System.NotImplementedException();
    }
/// <summary>
/// 执行固有行动
/// </summary>
/// <param name="inherentActionType"></param>
/// <exception cref="NotImplementedException"></exception>
    public void ExecutelnherentAction (E_InherentActionType  inherentActionType)
    {
        throw new System.NotImplementedException();
    }
/// <summary>
/// 执行许愿
/// </summary>
/// <param name="wishType"></param>
/// <exception cref="NotImplementedException"></exception>
    public void ExecuteWish(E_WishType  wishType)
    {
        throw new System.NotImplementedException();
    }
/// <summary>
/// 执行律
/// </summary>
/// <param name="lwaType"></param>
/// <exception cref="NotImplementedException"></exception>
    public void ExecuteLaw(E_LawType   lwaType)
    {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// 感觉每进入下一关得调用一下，把生命回满到10点，同时设置初始的欲望。比如第三关，玩家初始就有欲望。
    /// </summary>
    /// <param name="maxhp"></param>
    /// <param name="hp"></param>
    /// <param name="initialDesire"></param>
    public Player(int  initialDesire=0,int maxHp=10)
    {
        this.initialDesire = initialDesire;
        this.maxHp = maxHp;
        this.hp = maxHp;
    }
}
