using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Entity4 : Entity
{
  public override void InitEntity(int initialDesire = 0, int maxHp = 10)
    {
        AddBuff(E_BuffType.Desire, initialDesire);
        this.maxHp = maxHp;
        this.hp = maxHp;
    }

    public override void ManualInit()
    {
        base.ManualInit(); // 必须先调用父类，把自己注册进 BuffManager

        #region 状态管理器
        StateManager.Instance.ClearStates();

        StateManager.Instance.RegisterStateData(
            E_StateType_4.normal,
            new ActionNode[] {
                //new ActionNode(E_IntentType.Entity5_Equipoise,equipoise_Action_Equipoise),
            },
            new DesireNode[]
            {
                //new DesireNode(E_DesireType.Entity5_TheirWishes,equipoise_Desire_TheirWishes),
            }
        );
        
        StateManager.Instance.ChangeState(E_StateType_4.normal);
        #endregion

        // 最后进行数值初始化
        InitEntity(1, 36);
    }

    #region 行动

    public void CanItBeFurtherEnriched()
    {
        int length = 0;
        List<int> temp = new  List<int> { 0,1,2,3 };
        length+=GetBuff(E_BuffType.Want0)>0?0:1;
        length+=GetBuff(E_BuffType.Want1)>0?0:1;
        length+=GetBuff(E_BuffType.Want2)>0?0:1;
        length+=GetBuff(E_BuffType.Want3)>0?0:1;
        if (GetBuff(E_BuffType.Want0) > 0)
        {
            temp.Remove(0);
        }
        if (GetBuff(E_BuffType.Want1) > 0)
        {
            temp.Remove(1);
        }
        if (GetBuff(E_BuffType.Want2) > 0)
        {
            temp.Remove(2);
        }
        if (GetBuff(E_BuffType.Want3) > 0)
        {
            temp.Remove(3);
        }
        if (length > 0)
        {
            if(E_BuffType.TryParse($"Want{temp[Random.Range(0, length)]}", out E_BuffType want))
                AddBuff(want,1);
        }
    }

    public void WhyCanNotWePutDownMore()
    {
        EventManager.Instance.UnLockOption(E_OptionType.Level4_Option, 0);
    }
    #endregion

    #region 愿望

    public void IDonNotWantABrokenMirror()
    {
        for (int i = 1; i < 4; i++)
        {
            ProgressManager.Instance.TryGetPart(i, out Part part);
            if (!part.isDestroyed) continue;
            part.isDestroyed = false;
            part.hp=Math.Clamp(2*GetBuff(E_BuffType.Desire),1,part.maxHp);
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
            break;
        }
    }
    

    #endregion

}
