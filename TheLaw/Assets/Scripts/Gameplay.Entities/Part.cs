using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : CharacterBase
{
    public bool isDestroyed=false;
    public string name;
    public Entity owner;
    public override void Die()
    {
        this.isDestroyed = true;
        throw new System.NotImplementedException();
        //重写死亡的函数在里面增加部位破坏的逻辑，即hp将要变为零0时。
    }

    public override void BeAttacked(int atk)
    {
        base.BeAttacked(atk);
        owner.BeAttacked(atk);//攻击怪物部位，也会造成怪物本体扣血。
    }
}
