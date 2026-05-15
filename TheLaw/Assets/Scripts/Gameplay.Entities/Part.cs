using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : CharacterBase
{
    public int id;
    public override bool IsPlayer => false;
    public bool isDestroyed = false;
    public string partName;
    public Entity owner;
    public bool IsVisible = false;
    public override void Die()
    {
        this.isDestroyed = true;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        //重写死亡的函数在里面增加部位破坏的逻辑，即hp将要变为零0时。
    }

    public override void BeAttacked(int atk)
    {
        owner.BeAttacked(atk > hp?hp:atk);//攻击怪物部位，也会造成怪物本体扣血。
        base.BeAttacked(atk);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        
    }

    public override bool IsCouldBeAttacked()
    {
        return !isDestroyed;
    }

    public void AddHp(int heal)
    {
        hp = Math.Clamp(hp+heal,hp, maxHp);
    }
}
