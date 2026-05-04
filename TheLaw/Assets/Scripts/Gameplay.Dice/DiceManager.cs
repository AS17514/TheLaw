using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : ManagerBase<DiceManager>
{
    public Dictionary<E_DiceType, List<DiceBase>> dicePool = new Dictionary<E_DiceType, List<DiceBase>>
    {
        { E_DiceType.Time1, new List<DiceBase>() },
        { E_DiceType.Time2, new List<DiceBase>() },
        { E_DiceType.Time3, new List<DiceBase>() },
        { E_DiceType.Time4, new List<DiceBase>() },
        { E_DiceType.Action, new List<DiceBase>() },
        { E_DiceType.Mind,new List<DiceBase>() },
        { E_DiceType.Wild,new List<DiceBase>() }
    };
    public List<EntityDice> entityDicePool = new List<EntityDice>();

    public List<DiceBase> selectedDice = new List<DiceBase>();
    /// <summary>
    /// 向指定列表加骰子
    /// </summary>
    /// <param name="type"></param>
    public void AddDice(E_DiceType type, DiceBase dice = null)
    {
        if (dice != null)
        {
            this.dicePool[type].Add(dice);
        }
        else
        {
            switch (type)
            {
                case E_DiceType.Time1: this.dicePool[type].Add(new TimeDice()); break;
                case E_DiceType.Action: this.dicePool[type].Add(new ActionDice()); break;
                case E_DiceType.Mind: this.dicePool[type].Add(new MindDice()); break;
                case E_DiceType.Wild: this.dicePool[type].Add(new WildDice()); break;
            }
        }

        SortPoolByValue(type);
    }
    /// <summary>
    /// 向时间骰子列表加指定数量的时间骰子（每时间段开始要用）
    /// </summary>
    /// <param name="amount"></param>
    public void AddTimeDice(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            this.dicePool[E_DiceType.Time1].Add(new TimeDice());
        }

        SortPoolByValue(E_DiceType.Time1);
    }
    /// <summary>
    /// 获得池中某种骰子的个数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetDieCount(E_DiceType type)
    {
        return dicePool[type].Count;
    }
    /// <summary>
    /// 获得池中指定种类的骰子的个数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetDieCountByType(E_DiceType type)
    {
        return dicePool[type].Count;
    }
    /// <summary>
    /// 获得池中指定种类及点数的骰子的个数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetDieCountByType(E_DiceType type, int targetValue)
    {
        int count = 0;
        foreach (DiceBase dice in dicePool[type])
        {
            if (dice.value == targetValue)
            {
                count++;
            }
        }
        return count;
    }
    /// <summary>
    /// 获得池中指定种类、点数的骰子对象
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public DiceBase GetDie(E_DiceType type, int index)
    {
        return dicePool[type][index];
    }
    /// <summary>
    /// 重投指定种类、指定索引的骰子
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public void RerollDieAt(E_DiceType type, int index)
    {
        dicePool[type][index].Roll();
        SortPoolByValue(type);
    }
    /// <summary>
    /// 在传入的骰子种类数组中随机选择一种并获得
    /// </summary>
    /// <param name="types"></param>
    public void GetRandomDice(params E_DiceType[] types)
    {
        if (types == null || types.Length == 0) return;
        int count = types.Length;
        int i = Random.Range(0, count);
        AddDice(types[i]);
        SortPoolByValue(types[i]);
    }
    /// <summary>
    /// 移除特定类型下指定位置的骰子
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public void RemoveDie(E_DiceType type, int index)
    {
        if (index >= 0 && index < dicePool[type].Count)
        {
            dicePool[type].RemoveAt(index);
        }
        else
        {
            Debug.Log("DiceManager的RemoveDie索引越界");
        }
        SortPoolByValue(type);
    }
    /// <summary>
    /// 将指定骰子对象的值进行变动
    /// </summary>
    /// <param name="dice"></param>
    /// <param name="change"></param>
    public void ModifyDieValue(DiceBase dice, int change)
    {
        switch (dice.type)
        {
            case E_DiceType.Time1:
            case E_DiceType.Time2:
            case E_DiceType.Time3:
            case E_DiceType.Time4:
                E_DiceType tempType = dice.type;
                if (dice is TimeDice timeDice)
                {
                    timeDice.ChangeTimeDiceValue(change);
                }
                AddDice(dice.type, dice);
                dicePool[tempType].Remove(dice);
                break;
            case E_DiceType.Action:
            case E_DiceType.Mind:
                int newValue = dice.value + change;
                dice.value = Mathf.Clamp(newValue, 1, 6);
                SortPoolByValue(dice.type);
                break;
            case E_DiceType.Wild:
                break;

        }
    }
    /// <summary>
    /// 将指定骰对象转化为另种类型
    /// </summary>
    /// <param name="dice"></param>
    /// <param name="newType"></param>
    public void TransformDie(DiceBase dice, E_DiceType newType)
    {
        E_DiceType tempType = dice.type;
        //不会有能够直接把别的类型的骰子转化出时间骰子的能力
        dice.type = newType;
        dicePool[newType].Add(dice);
        dicePool[tempType].Remove(dice);
        // SortPoolByValue(tempType);
        // SortPoolByValue(dice.type);
        if (newType == E_DiceType.Wild)
        {
            if (dice is WildDice wildDice)
                wildDice.Renew();
        }
        SortPoolByValue(tempType);
        SortPoolByValue(dice.type);
    }

    /// <summary>
    /// 判断选中骰子是否满足条件
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    public bool IsSelectionValid(params DiceCondition[] conditions)
    {
        return IsSelectionValid(0, conditions);
    }

    /// <summary>
    /// 判断选中骰子是否满足条件
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    public bool IsSelectionValid(int specialAddLength, params DiceCondition[] conditions)
    {
        if (conditions == null || conditions.Length == 0) return false;
        if (selectedDice == null || selectedDice.Count == 0) return false;

        if ((conditions.Length + specialAddLength) != selectedDice.Count)
        {
            Debug.Log("老大，selectedDice数量与conditions数量对不上喵");
            return false;
        }

        // 全局重置状态
        foreach (var dice in selectedDice) dice.isValid = false;
        bool result = GlobalDFS(0, conditions, selectedDice);
        if (result == false)
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
        return result;
    }
    /// <summary>
    /// 一个内部辅助方法，用来迭代穷举所有选项的可能性,比较结束之后返回一个bool值,IsSelectionValid使用
    /// </summary>
    /// <param name="currentConditionIndex"></param>
    /// <param name="conditions"></param>
    /// <param name="availableDice"></param>
    /// <returns></returns>
    private bool GlobalDFS(int currentConditionIndex, DiceCondition[] conditions, List<DiceBase> availableDice)
    {
        if (currentConditionIndex >= conditions.Length) return true;

        DiceCondition condition = conditions[currentConditionIndex];

        for (int i = 0; i < availableDice.Count; i++)
        {
            DiceBase dice = availableDice[i];

            // 判断类型是否匹配（或者是万能骰子）
            // 1. 常规判断：类型是否严格匹配（或者是万能骰子）
            bool isTypeMatch = (dice.type == condition.type || dice.type == E_DiceType.Wild);
            //如果条件是要“行动或思维”，那只要骰子是行动或思维就放行
            if (condition.type == E_DiceType.ActionOrMind)
            {
                isTypeMatch = (dice.type == E_DiceType.Action || dice.type == E_DiceType.Mind || dice.type == E_DiceType.Wild);
            }
            // 2. 【新增的微调逻辑：时间骰子豁免权】
            // 如果常规判断没通过，我们额外检查一下是不是“时间通用”的情况
            if (!isTypeMatch)
            {
                // 判断这个技能条件是否在索要时间骰子（不管索要的是1还是4）
                bool isConditionTime = (condition.type == E_DiceType.TimeAny);

                // 判断当前拿来核对的这个骰子，是不是时间骰子
                bool isDiceTime = (dice.type == E_DiceType.Time1 || dice.type == E_DiceType.Time2 ||
                                   dice.type == E_DiceType.Time3 || dice.type == E_DiceType.Time4);

                // 如果技能要的是时间，玩家塞进来的也是时间，强行让保安放行
                if (isConditionTime && isDiceTime)
                {
                    isTypeMatch = true;
                }
            }

            if (dice.isValid == false && isTypeMatch)
            {
                // 如果是万能骰子，直接判定数值通过；否则进行数学比较
                bool isValueMatch = (dice.type == E_DiceType.Wild) || CompareToMath(condition.mode, condition.value, dice.value);

                if (isValueMatch)
                {
                    dice.isValid = true; // 做出选择

                    // 递归匹配下一个条件
                    if (GlobalDFS(currentConditionIndex + 1, conditions, availableDice))
                    {
                        return true; // 如果后续全通了，直接返回
                    }

                    dice.isValid = false;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 一个内部辅助方法，用来把比较的枚举转化成对应的数学模式,比较结束之后返回一个bool值,IsSelectionValid使用
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="require">需要用来比的条件需要用来比的条件</param>
    /// <param name="value">被拿来做比较的值</param>
    private bool CompareToMath(E_CompareType mode, int require, int value)
    {
        bool result = false;
        switch (mode)
        {
            case E_CompareType.Equal:
                if (require == value)
                    result = true;
                break;
            case E_CompareType.Greater:
                if (value > require)
                    result = true;
                break;
            case E_CompareType.Less:
                if (value < require)
                    result = true;
                break;
            case E_CompareType.Any:
                result = true;
                break;
            default:
                Debug.Log("DiceManageer的CompareToMath问题喵");
                break;
        }
        if (result == false)
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
        return result;
    }

    /// <summary>
    /// 按顺序整理选中骰子列表,先行动，再思维，再时间，再万能,每种类型内部的顺序是从小到大。
    /// </summary>
    public void SortSelectedByValue()
    {
        if (selectedDice == null || selectedDice.Count == 0) return;
        selectedDice.Sort((a, b) =>
            {
                // 1. 获取两者的类型权重
                int weightA = GetTypeWeight(a.type);
                int weightB = GetTypeWeight(b.type);

                // 2. 如果类型权重不同，按权重升序排（Action最前）
                if (weightA != weightB)
                {
                    return weightA.CompareTo(weightB);
                }

                // 3. 如果类型相同，按点数从小到大排
                return a.value.CompareTo(b.value);
            });
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice);
    }
    /// <summary>
    ///  一个内部辅助方法，用来定义优先级,SortSelectedByValue使用
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private int GetTypeWeight(E_DiceType type)
    {
        switch (type)
        {
            case E_DiceType.Action: return 0;
            case E_DiceType.Mind: return 1;
            // 所有的 Time 类型权重都一样
            case E_DiceType.Time1:
            case E_DiceType.Time2:
            case E_DiceType.Time3:
            case E_DiceType.Time4: return 2;
            case E_DiceType.Wild: return 3;
            default: return 99; // 未知类型排最后
        }
    }
    /// <summary>
    /// 清空骰子池
    /// </summary>
    public void ClearPool()
    {
        foreach (var list in dicePool.Values)
        {
            list.Clear();
        }
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice);
        entityDicePool.Clear();
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice, entityDicePool);
    }
    /// <summary>
    /// 清空选中骰子
    /// </summary>
    public void ClearSelected()
    {
        selectedDice.Clear();
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice);
    }
    /// <summary>
    /// 把某一类骰子的列表按从小到大排序，并初始化或更新索引值index
    /// </summary>
    /// <param name="type"></param>
    public void SortPoolByValue(E_DiceType type)
    {
        if (!dicePool.ContainsKey(type) || dicePool[type] == null || dicePool[type].Count == 0)
        {
            return;
        }
        dicePool[type].Sort((a, b) => a.value.CompareTo(b.value));
        for (int i = 0; i < dicePool[type].Count; i++)
        {
            dicePool[type][i].index = i;
        }

        switch (type)
        {
            case E_DiceType.Action:
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_ActionDice, dicePool[E_DiceType.Action]);
                break;
            case E_DiceType.Mind:
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_MindDice, dicePool[E_DiceType.Mind]);
                break;
            case E_DiceType.Time1:
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDice1Count, dicePool[type].Count);
                break;
            case E_DiceType.Time2:
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDice2Count, dicePool[type].Count);
                break;
            case E_DiceType.Time3:
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDice3Count, dicePool[type].Count);
                break;
            case E_DiceType.Time4:
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDice4Count, dicePool[type].Count);
                break;
            case E_DiceType.Wild:
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WildDiceCount, dicePool[type].Count);
                break;
        }
    }
    //E_ComboType
    //MindActionPair, // 1思1行 (点数相同)
    //DoubleMind,     // 2思 (点数相同)
    //DoubleAction,   // 2行 (点数相同)
    //Quadruple       // 4个思/行 (点数相同)
    /// <summary>
    /// IsSelectionValid的重构，用来实现第三关的判断点数是否相同
    /// </summary>
    /// <param name="comboType"></param>
    /// <returns></returns>
    public bool IsSelectionValid(E_ComboType comboType)
    {
        if (selectedDice == null || selectedDice.Count == 0) return false;

        // 分支4需要4个骰子，其余需要2个
        int requiredCount = (comboType == E_ComboType.Quadruple) ? 4 : 2;
        if (selectedDice.Count != requiredCount) return false;

        // 验证前重置状态
        foreach (var dice in selectedDice) dice.isValid = false;

        int actionCount = 0;
        int mindCount = 0;
        int targetValue = -1; // 记录基准点数

        foreach (var dice in selectedDice)
        {
            // 防止时间骰子混进来
            if (dice.type == E_DiceType.Time1 || dice.type == E_DiceType.Time2 ||
                dice.type == E_DiceType.Time3 || dice.type == E_DiceType.Time4)
            {
                return false;
            }

            if (dice.type == E_DiceType.Action) actionCount++;
            if (dice.type == E_DiceType.Mind) mindCount++;

            // 如果不是万能骰子，则用来确定或核对基准点数
            if (dice.type != E_DiceType.Wild)
            {
                if (targetValue == -1)
                {
                    targetValue = dice.value; // 第一颗非万能骰子定下基准点数
                }
                else if (targetValue != dice.value)
                {
                    return false; // 发现点数不一致的，直接判定失败
                }
            }
        }

        // 根据类型进行最终判定
        bool isMatch = false;
        switch (comboType)
        {
            case E_ComboType.MindActionPair:
                // 需要 1行1思。如果是 2行 或者 2思 就失败。
                // 只要行动<=1，且思维<=1，剩余哪怕全是万能骰子也能完美变成1行1思！
                isMatch = (actionCount <= 1 && mindCount <= 1);
                break;
            case E_ComboType.DoubleMind:
                // 需要 2思。不能有行动骰子混进来
                isMatch = (actionCount == 0);
                break;
            case E_ComboType.DoubleAction:
                // 需要 2行。不能有思维骰子混进来
                isMatch = (mindCount == 0);
                break;
            case E_ComboType.Quadruple:
                // 需要 4个。只要上面没被时间骰子或点数不同给拦截，能到这一步就已经匹配成功了
                isMatch = true;
                break;
        }

        // 如果匹配成功，全部标记为已使用
        if (isMatch)
        {
            foreach (var dice in selectedDice) dice.isValid = true;
            return true;
        }

        return false;
    }

    public int GetSelectedTime1DiceCount()
    {
        int Count = 0;
        foreach (var dice in selectedDice)
        {
            if (dice.type == E_DiceType.Time1)

            {
                Count++;
            }
        }
        return Count;
    }

    public int GetSelectedTime2DiceCount()
    {

        int Count = 0;
        foreach (var dice in selectedDice)
        {
            if (dice.type == E_DiceType.Time2)

            {
                Count++;
            }
        }
        return Count;
    }

    public int GetSelectedTime3DiceCount()
    {
        int Count = 0;
        foreach (var dice in selectedDice)
        {
            if (dice.type == E_DiceType.Time3)

            {
                Count++;
            }
        }
        return Count;
    }

    public int GetSelectedTime4DiceCount()
    {
        int Count = 0;
        foreach (var dice in selectedDice)
        {
            if (dice.type == E_DiceType.Time4)

            {
                Count++;
            }
        }
        return Count;
    }

    public int GetSelectedWildDiceCount()
    {
        int Count = 0;
        foreach (var dice in selectedDice)
        {
            if (dice.type == E_DiceType.Wild)

            {
                Count++;
            }
        }
        return Count;
    }



    public List<DiceBase> UpdateSelectedDice()
    {
        return selectedDice;
    }

    public List<EntityDice> UpdateEntityDice()
    {
        return entityDicePool;
    }

    /// <summary>
    /// 消耗掉选中的、且通过验证的骰子
    /// </summary>
    public void ConsumeValidSelectedDice(bool isResponse = false)
    {
        // 倒序遍历或者克隆一个列表遍历，防止在遍历过程中移除元素导致索引错乱
        List<DiceBase> dicesToConsume = new List<DiceBase>();
        foreach (var dice in selectedDice)
        {
            if (dice.isValid)
            {
                dicesToConsume.Add(dice);
            }
        }

        // 从总池子里正式移除
        foreach (var dice in dicesToConsume)
        {
            dicePool[dice.type].Remove(dice);
            SortPoolByValue(dice.type); // 更新池子和触发UI刷新

            // 如果你的骰子在场景中有实际的 GameObject，这里可能还需要销毁它
            // GameObject.Destroy(dice.gameObject);
        }

        // 最后清空购物车
        ClearSelected();

        if (!isResponse)
        {
            EventCenter.Instance.EventTrigger(E_EventType.Logic_PlayerActionExecuted);
        }
    }


    /// <summary>
    /// 获取选中列表中未被作为消耗（isValid == false）的“目标”骰子
    /// 注意：必须在调用 ConsumeValidSelectedDice 之前调用！
    /// </summary>
    /// <returns>目标骰子列表</returns>
    public List<DiceBase> GetTargetDiceFromSelection()
    {
        List<DiceBase> targetDices = new List<DiceBase>();
        if (selectedDice == null || selectedDice.Count == 0) return targetDices;

        foreach (var dice in selectedDice)
        {
            // 在 IsSelectionValid 中没有被验证通过（扣除）的骰子，即为操作目标
            if (!dice.isValid)
            {
                targetDices.Add(dice);
            }
        }
        return targetDices;
    }

    /// <summary>
    /// 不能删
    /// </summary>
    private DiceManager()
    {

    }

    #region 怪物骰子逻辑

    /// <summary>
    /// 向实体/怪物骰子池添加骰子
    /// </summary>
    public void AddEntityDice(EntityDice dice = null)
    {
        if (dice != null)
        {
            entityDicePool.Add(dice);
        }
        else
        {
            entityDicePool.Add(new EntityDice());
        }
        SortEntityPoolByValue();
    }

    /// <summary>
    /// 修改实体骰子的点数
    /// </summary>
    public void ModifyEntityDieValue(EntityDice dice, int change)
    {
        if (entityDicePool.Contains(dice))
        {
            int newValue = dice.value + change;
            dice.value = Mathf.Clamp(newValue, 1, 6); // 假设怪物骰子也是 1-6 点
            SortEntityPoolByValue();
        }
        else
        {
            Debug.LogWarning("该骰子不在 entityDicePool 中！");
        }
    }

    /// <summary>
    /// 移除实体骰子池中指定位置的骰子
    /// </summary>
    public void RemoveEntityDie(int index)
    {
        if (index >= 0 && index < entityDicePool.Count)
        {
            entityDicePool.RemoveAt(index);
            SortEntityPoolByValue();
        }
        else
        {
            Debug.Log("DiceManager的RemoveEntityDie索引越界");
        }
    }

    /// <summary>
    /// 整理实体骰子池并更新索引
    /// </summary>
    public void SortEntityPoolByValue()
    {
        if (entityDicePool == null || entityDicePool.Count == 0) return;

        // 按点数从小到大排
        entityDicePool.Sort((a, b) => a.value.CompareTo(b.value));

        // 更新索引
        for (int i = 0; i < entityDicePool.Count; i++)
        {
            entityDicePool[i].index = i;
        }

        // 触发实体骰子的UI刷新事件
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice, entityDicePool);
    }
    /// <summary>
    /// 清空怪物骰子池
    /// </summary>
    public void ClearEntityPool()
    {
        entityDicePool.Clear();
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice, entityDicePool);
    }
    /// <summary>
    /// 第一关怪物骰子池专用方法，去掉那些已经被应对的骰子。
    /// </summary>
    public void ConsumeValidEntityDice()
    {
        List<EntityDice> dicesToConsume = new List<EntityDice>();
        foreach (var dice in entityDicePool)
        {
            if (dice.isValid)
            {
                dicesToConsume.Add(dice);
            }
        }

        // 从总池子里正式移除
        foreach (var dice in dicesToConsume)
        {
            entityDicePool.Remove(dice);
            SortEntityPoolByValue();
        }
    }
    #endregion
}
