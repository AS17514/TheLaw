using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Entity4 : Entity
{
    public override int GetDamageReduction()
        => GetBuff(E_BuffType.Want0) > 0 ? 1 : 0;

    public override int GetDamageBonus()
        => GetBuff(E_BuffType.Want2) > 0 ? 1 : 0;

    public override int GetTimeProgressModifier()
        => GetBuff(E_BuffType.Want1) > 0 ? -1 : 0;

    public override int GetPhaseDiceModifier()
        => GetBuff(E_BuffType.Want3) > 0 ? -1 : 0;
    
    public bool IsDestroyed=false;
    public int CountMissingWants()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            string name = $"Want{i}";
            if (E_BuffType.TryParse(name, out E_BuffType type) && GetBuff(type) <= 0)
                count++;
        }
        return count;
    }
  public override void InitEntity(int initialDesire = 0, int maxHp = 10)
    {
        AddBuff(E_BuffType.Desire, initialDesire);
        this.maxHp = maxHp;
        this.hp = maxHp;
    }

    public override void Die()
    {
        AddBuff(E_BuffType.Desire,-1);
        ProgressManager.Instance.PartStateChange(0,false);
        IsDestroyed = true;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        base.Die();
    }

    public override void ManualInit()
    {
        base.ManualInit(); // 必须先调用父类，把自己注册进 BuffManager
        ProgressManager.Instance.PartStateChange(0,true);
        #region 状态管理器
        StateManager.Instance.ClearStates();

        StateManager.Instance.RegisterStateData(
            E_StateType_4.normal,
            new ActionNode[] {
                new ActionNode(GetCurrentAction(out Action action),action),
            },
            new DesireNode[]
            {
                new DesireNode(GetCurrentDesire(out Action desire),desire),
            }
        );
        StateManager.Instance.ChangeState(E_StateType_4.normal);
        
        EventCenter.Instance.AddEventListener(E_EventType.UI_Update_EntityBuff,AddEventListenerToBuff);
        
        #endregion

        // 最后进行数值初始化
        InitEntity(1, 36);
    }
    
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.UI_Update_EntityBuff, AddEventListenerToBuff);
    }

    public void AddEventListenerToBuff(object info = null)
    {
        StateManager.Instance.RegisterStateData(
            E_StateType_4.normal,
            new ActionNode[] {
                new ActionNode(GetCurrentAction(out Action action),action),
            },
            new DesireNode[]
            {
                new DesireNode(GetCurrentDesire(out Action desire),desire),
            }
        );
    }

    public override void BeAttacked(int atk)
    {
        base.BeAttacked(atk- GetDamageReduction());
    }

    #region 行动

    public E_IntentType GetCurrentAction(out Action action)
    {
        int length = CountMissingWants();
        if (length < GetBuff(E_BuffType.Desire))
        {
            action=CanItBeFurtherEnriched+AddWantToAction();
            return E_IntentType.Entity4_CanItBeFurtherEnriched;
        }
        else 
        {
            action= WhyCanNotWePutDownMore+AddWantToAction();
            return  E_IntentType.Entity4_WhyCanNotWePutDownLess;
        }
    }
    
    
    public void CanItBeFurtherEnriched()
    {
        int length = CountMissingWants();
        List<int> temp = new  List<int> { 0,1,2,3 };
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
        if (IsDestroyed)
            temp.Remove(0);
        if (ProgressManager.Instance.TryGetPart(1, out Part p1) && p1 != null && p1.isDestroyed)
            temp.Remove(1);
        if (ProgressManager.Instance.TryGetPart(2, out Part p2) && p2 != null && p2.isDestroyed)
            temp.Remove(2);
        if (ProgressManager.Instance.TryGetPart(3, out Part p3) && p3 != null && p3.isDestroyed)
            temp.Remove(3);
        if (temp.Count > 0)
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

    public E_DesireType GetCurrentDesire(out Action action)
    {
        bool isHadPartBroken = false;
        foreach (var part in parts)
        {
            if(part.isDestroyed)
            {
                isHadPartBroken = true;
                break;
            }
        }
        if (isHadPartBroken)
        {
            action=IDonNotWantABrokenMirror;
            return E_DesireType.Entity4_IDonNotWantABrokenMirror;
        }
        else
        {
            action= IDonNotWantALifeThatRemainsUnchanged;
            return  E_DesireType.Entity4_IDonNotWantALifeThatRemainsUnchanged;
        }
    }
    public void IDonNotWantABrokenMirror()
    {
        for (int i = 1; i < 4; i++)
        {
            ProgressManager.Instance.TryGetPart(i, out Part part);
            if (!part.isDestroyed) continue;
            part.isDestroyed = false;
            part.hp=Math.Clamp(2*GetBuff(E_BuffType.Desire),1,part.maxHp);
            if(GetBuff(E_BuffType.Desire) <=1)
            {
                AddBuff(E_BuffType.Desire, 2);
                EventManager.Instance.UnLockOption(E_OptionType.Level4_Option, 11);
            }
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
            break;
        }
    }

    public void IDonNotWantALifeThatRemainsUnchanged()
    {
        int length = 0;
        length+=GetBuff(E_BuffType.Want1)>0?0:1;
        length+=GetBuff(E_BuffType.Want2)>0?0:2;
        length+=GetBuff(E_BuffType.Want3)>0?0:3;
        switch (Random.Range(0, 4))
        {
            case 0:
                Want0(length);
                break;
            case 1:
                Want1(length);
                break;
            case 2:
                for(int i=0;i<length;i++)
                {
                    Want2();
                }
                break;
            case 3:
                for(int i=0;i<length;i++)
                {
                    Want3();
                }
                break;
        }
    }
    

    #endregion

    #region 渴望

    private Action AddWantToAction()
    {
        Action action = null;
        if (GetBuff(E_BuffType.Want0) > 0)
            action += ()=>Want0();
        if (GetBuff(E_BuffType.Want1) > 0)
            action += ()=>Want1();
        if (GetBuff(E_BuffType.Want2) > 0)
            action +=Want2;
        if (GetBuff(E_BuffType.Want3) > 0)
            action +=Want3;
        return action;
    }
    
    
    public void Want0(int i=1)
    {
        ProgressManager.Instance.player.BeAttacked(i);
    }
    public void Want1(int i=1)
    {
        for (int j = 1; j < 5; j++)
        {
            if (ProgressManager.Instance.TryGetPart(j, out Part part) && part != null)
                part.AddHp(i);
        }
    }
    public void Want2()
    {
        // 1. 把行动和思维骰子池合并到一个临时列表
        List<DiceBase> eligibleDice = new List<DiceBase>();
        eligibleDice.AddRange(DiceManager.Instance.dicePool[E_DiceType.Action]);
        eligibleDice.AddRange(DiceManager.Instance.dicePool[E_DiceType.Mind]);

        // 2. 确保池子里至少有一个骰子
        if (eligibleDice.Count > 0)
        {
            // 3. 随机抽取一个目标
            int randomIndex = Random.Range(0, eligibleDice.Count);
            DiceBase target = eligibleDice[randomIndex];
    
            // 4. 调用 DiceManager 从真实骰子池中将其彻底删除
            // 注：RemoveDie 会在内部自动调用 SortPoolByValue 刷新剩余骰子的 index
            DiceManager.Instance.RemoveDie(target.type, target.index);
        }
    }
    public void Want3()
    {
        E_DiceType[] timeTypes = { E_DiceType.Time4, E_DiceType.Time3, E_DiceType.Time2, E_DiceType.Time1 };
        foreach (var type in timeTypes)
        foreach (var dice in new List<DiceBase>(DiceManager.Instance.dicePool[type]))
             DiceManager.Instance.ModifyDieValue(dice, 1);
    }
    
    #endregion
}
