using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity3 : Entity
{
public override void InitEntity(int initialDesire = 0, int maxHp = 10)
    {
        AddBuff(E_BuffType.Desire,initialDesire);
        this.maxHp = maxHp;
        this.hp = maxHp;
    }
    
    public override void ManualInit()
    {
        base.ManualInit(); // 必须先调用父类，把自己注册进 BuffManager

        #region 状态管理器
        StateManager.Instance.ClearStates();

        StateManager.Instance.RegisterStateData(
            E_StateType_3.normal, 
            new ActionNode[]{ 
                new ActionNode(E_IntentType.Entity3_Marble,normal_Action_Marble),
            },
            new DesireNode[]
            {
                //new DesireNode(E_DesireType.Entity2_HiddenBehindTheClothes,normal_Desire_HiddenBehindTheClothes),
                
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_3.weightless, 
            new ActionNode[]{ 
                //new ActionNode(E_IntentType.Entity2_Stress, ashamed_Action_Stress)
            },
            new DesireNode[]
            {
                //new DesireNode(E_DesireType.Entity2_IfThatCountsAsMyClothesToo,ashamed_Desire_IfThatCountsAsMyClothesToo),
                //new DesireNode(E_DesireType.Entity2_IfThoseCouldBeSofter,ashamed_Desire_IfThoseCouldBeSofter)
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_3.free_notfree,
            new ActionNode[]
            {
                //new ActionNode(E_IntentType.Entity2_HysterialStress,hysterial_Action_HysterialStress)
            }, 
            new DesireNode[]
            {
                //new DesireNode(E_DesireType.Entity2_PleaseTearThoseTornTattersApart,hysterial_Desire_PleaseTearThoseTornTattersApart)
            }
        );
        

        StateManager.Instance.ChangeState(E_StateType_3.normal);
        #endregion

        // 最后进行数值初始化
        InitEntity(0, 27);
    }

    #region 行动

    public void normal_Action_Marble()
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                 - ProgressManager.Instance.player.GetBuff(E_BuffType
                                                                     .Desire)));
            if(e3.GetBuff(E_BuffType.Desire)
                - ProgressManager.Instance.player.GetBuff(E_BuffType
                    .Desire)<0)//若此时对象的“欲望”更大则无法应对
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][0].IsVisible = true;
        }
        
        
    }
    

    #endregion

    #region 欲望

    public void normal_Desire_RegardingTheSharpStonesOnTheRiverbank()
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            e3.AddBuff(E_BuffType.Desire, 1);
        ProgressManager.Instance.player.BeAttacked(1);
    }
    public void normal_Desire_RegardingTheWitheringFlowersOnTheRoadside()
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            e3.AddBuff(E_BuffType.Desire, 2);
        ProgressManager.Instance.AddTimeProgress(-2);
    }
    public void normal_Desire_RegardingOccasionalShuttleCars()
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            e3.AddBuff(E_BuffType.Desire, 2);
        ProgressManager.Instance.AddTimeProgress(-2);
    }
    

    #endregion
}
