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
        {E_DiceType.Action,new List<DiceBase>() },
        { E_DiceType.Mind,new List<DiceBase>() },
        { E_DiceType.Wild,new List<DiceBase>() }
};

public List<DiceBase> selectedDice=new List<DiceBase>();
    /// <summary>
    /// 向指定列表加股子
    /// </summary>
    /// <param name="type"></param>
    public void AddDice(E_DiceType type,DiceBase dice=null)
    {
        switch (type)
        {
            case E_DiceType.Time1:
                this.dicePool[type].Add(new TimeDice());
                break;
            case E_DiceType.Time2:
            case E_DiceType.Time3:
            case E_DiceType.Time4:
                if (dice != null)
                    this.dicePool[type].Add(dice);
                break;
            case E_DiceType.Action:
                this.dicePool[type].Add(new ActionDice());
                break;
            case E_DiceType.Mind:
                this.dicePool[type].Add(new MindDice());
                break;
            case E_DiceType.Wild:
                this.dicePool[type].Add(new WildDice());
                break;
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
/// 获得池中某种般子的个数
/// </summary>
/// <param name="type"></param>
/// <returns></returns>
    public int GetDieCount(E_DiceType type)
    {
        return dicePool[type].Count;
    }
/// <summary>
/// 获得池中指定种类的般子的个数
/// </summary>
/// <param name="type"></param>
/// <returns></returns>
    public int GetDieCountByType(E_DiceType type)
    {
        return dicePool[type].Count;
    }
/// <summary>
/// 获得池中指定种类及点数的般子的个数
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
/// 获得池中指定种类、点数的般子对象
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
/// 在传入的般子种类数组中随机选择一种并获得
/// </summary>
/// <param name="types"></param>
    public void GetRandomDice(params E_DiceType[] types)
    {
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
            Debug.Log("Dicemanager的RemoveDie索引越界");
        }
        SortPoolByValue(type);
    }

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
                AddDice(dice.type,dice);
                dicePool[tempType].Remove(dice);
                break;
            case E_DiceType.Action:
            case E_DiceType.Mind:
                int newValue=dice.value+change;
                dice.value=Mathf.Clamp(newValue, 1, 6);
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
        SortPoolByValue(tempType);
        SortPoolByValue(dice.type);
        if (newType == E_DiceType.Wild)
        {
            if(dice is WildDice wildDice)
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
        if (conditions == null || conditions.Length == 0)
        {
            Debug.Log("conditions为空");
            return false;
        }

        if (selectedDice == null || selectedDice.Count == 0)
        {
            Debug.Log("selectedDice为空");
            return false;
        }

        if (conditions.Length != selectedDice.Count)
        {
            Debug.Log("老大，selectedDice数量与conditions数量对不上喵");
            return false;
        }
        int count = conditions.Length;
        List<DiceBase> timeDiceList = new List<DiceBase>();
        List<DiceBase> actionDiceList = new List<DiceBase>();
        List<DiceBase> mindDiceList = new List<DiceBase>();
        List<DiceBase> wildDiceList = new List<DiceBase>();
        foreach (var dice in selectedDice)
        {
            switch (dice.type)
            {
                case E_DiceType.Action:
                    actionDiceList.Add(dice);
                    break;
                case E_DiceType.Mind:
                    mindDiceList.Add(dice);
                    break;
                case E_DiceType.Time1:
                case E_DiceType.Time2:
                case E_DiceType.Time3:
                case E_DiceType.Time4:
                    timeDiceList.Add(dice);
                    break;
                case E_DiceType.Wild:
                    wildDiceList.Add(dice);
                    break;
                default:
                    Debug.Log("DiceManager的IsSelectionValid有问题喵");
                    break;
            }
        }
        int actionConditionsLength = 0;
        int mindConditionsLength = 0;
        int wildConditionsLength = 0;
        int timeConditionsLength = 0;
        List<DiceCondition> actionConditions=new List<DiceCondition>();
        List<DiceCondition> mindConditions = new List<DiceCondition>();
        List<DiceCondition> wildConditions = new List<DiceCondition>();
        List<DiceCondition> timeConditions = new List<DiceCondition>();
        foreach (DiceCondition condition in conditions)
        {
            switch (condition.type)
            {
                case E_DiceType.Action:
                    actionConditionsLength++;
                    actionConditions.Add(condition);
                    break;
                case E_DiceType.Mind:
                    mindConditionsLength++;
                    mindConditions.Add(condition);
                    break;
                case E_DiceType.Time1:
                case E_DiceType.Time2:
                case E_DiceType.Time3:
                case E_DiceType.Time4:
                    timeConditionsLength++;
                    timeConditions.Add(condition);
                    break;
                case E_DiceType.Wild:
                    wildConditionsLength++;
                    wildConditions.Add(condition);
                    break;
            }
        }
        DiceCondition[] actionConditionsArray =actionConditions.ToArray();
        DiceCondition[] mindConditionsArray = mindConditions.ToArray();
        DiceCondition[] wildConditionsArray = wildConditions.ToArray();
        DiceCondition[] timeConditionsArray = timeConditions.ToArray();
        bool[] actionOption=new bool[actionConditionsLength];
        bool[] mindOption=new bool[mindConditionsLength];
        bool[] wildOption=new bool[wildConditionsLength];
        bool[] timeOption=new bool[timeConditionsLength];
        int[] index=new int[]{actionConditionsLength,mindConditionsLength,timeConditionsLength,wildConditionsLength};
        for (int i =0;i<actionConditionsLength;i++)
        {
            actionOption[i]=false;
        }

        for (int i = 0; i < mindConditionsLength; i++)
        {
            mindOption[i]=false;
        }

        for (int i = 0; i < wildConditionsLength; i++)
        {
            wildOption[i]=false;
        }

        for (int i = 0; i < timeConditionsLength; i++)
        {
            timeOption[i]=false;
        }
        bool result1 = false;
        bool result2 = false;
        bool result3 = false;
        bool result4 = false;
        if (actionConditionsLength > 0)
        {
            result1 = BackTrack(actionOption, index, actionConditionsArray);
        }
        else
        {
            result1 = true;
        }
        if (mindConditionsLength > 0)
        {
            result2 = BackTrack(mindOption, index, mindConditionsArray);;
        }
        else
        {
            result2 = true;
        }
        if (timeConditionsLength > 0)
        {
            result3 = BackTrack(timeOption, index, timeConditionsArray);;
        }
        else
        {
            result3 = true;
        }
        if (wildConditionsLength > 0)
        {
            result4 = BackTrack(wildOption, index, wildConditionsArray);;
        }
        else
        {
            result4 = true;
        }
        return result1 && result2 && result3 && result4;
    }
/// <summary>
/// 一个内部辅助方法，用来把比较的枚举转化成对应的数学模式,比较结束之后返回一个bool值,IsSelectionValid使用
/// </summary>
/// <param name="mode"></param>
/// <param name="requare">需要用来比的条件需要用来比的条件</param>
/// <param name="value">被拿来做比较的值</param>
    private bool CompareToMath(E_CompareType mode,int require,int value)
    {
        bool result = false;
        switch (mode)
        {
            case E_CompareType.Equal:
                if(require == value)
                result = true;
                break;  
            case E_CompareType.Greater:
                if(value > require)
                result = true;
                break;  
            case E_CompareType.Less:
                if(value < require)
                result = true;
                break;
            case E_CompareType.Any:
                result = true;
                break;
            default:
                Debug.Log("DiceManageer的CompareToMath问题喵");
                break;
        }
        return result;
    }
/// <summary>
/// 一个内部辅助方法，用来迭代穷举所有选项的可能性，并且判断能不能用,比较结束之后返回一个bool值,IsSelectionValid使用
/// </summary>
/// <param name="options"></param>
/// <param name="conditions">某一种类的条件，比如说所有涉及行动骰子的条件。</param>
/// <param name="tempIndex">长度必须为4，是IsSelectionValid中的int[] index</param>
    private bool BackTrack(bool[] options,int[] tempIndex,params DiceCondition[] conditions)
    {
        //
        if (tempIndex == null || tempIndex.Length == 0||tempIndex.Length>4)
        {
            return false;
        }
        if (conditions == null || conditions.Length == 0)
            return false;
        if (options == null || options.Length == 0)
            return false;
        if(options.Length!=conditions.Length)
            return false;
        //
        //
        List<DiceBase> tempDiceList = new List<DiceBase>();
        switch (conditions[0].type)
        {
            case E_DiceType.Action:
                List<DiceBase> tempActionDiceList = selectedDice.GetRange(0, tempIndex[0]);
                tempDiceList=tempActionDiceList;
                break;
            case E_DiceType.Mind:
                List<DiceBase> tempMindDiceList = selectedDice.GetRange(tempIndex[0], tempIndex[1]);
                tempDiceList=tempMindDiceList;
                break;
            case E_DiceType.Time1:
            case E_DiceType.Time2:
            case E_DiceType.Time3:
            case E_DiceType.Time4:
                List<DiceBase> tempTimeDiceList = selectedDice.GetRange(tempIndex[0]+tempIndex[1], tempIndex[2]);
                tempDiceList=tempTimeDiceList;
                break;
            case E_DiceType.Wild:
                List<DiceBase> tempWildDiceList = selectedDice.GetRange(tempIndex[0]+tempIndex[1]+tempIndex[2], tempIndex[3]);
                tempDiceList=tempWildDiceList;
                break;
        }
        foreach (var dice in tempDiceList)
        {
            dice.isValid = false; 
        }
        return DFS(0, conditions, tempDiceList);
    }
/// <summary>
/// 一个内部辅助方法，用来具体帮助BackTrack实现迭代穷举所有选项的可能性,比较结束之后返回一个bool值,IsSelectionValid的BackTrack使用
/// </summary>
/// <param name="currentConditionIndex"></param>
/// <param name="conditions"></param>
/// <param name="availableDice"></param>
/// <returns></returns>
    private bool DFS(int currentConditionIndex, DiceCondition[] conditions, List<DiceBase> availableDice)
    {
        if (currentConditionIndex >= conditions.Length)
        {
            return true;
        }
        DiceCondition condition = conditions[currentConditionIndex];
        for (int i = 0; i < availableDice.Count; i++)
        {
            DiceBase dice = availableDice[i];
            if (dice.isValid == false && CompareToMath(condition.mode, condition.value, dice.value))
            {
                dice.isValid = true;
                if (DFS(currentConditionIndex + 1, conditions, availableDice))
                {
                    return true;
                }
                dice.isValid = false;
            }

        }
        return false;
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
            case E_DiceType.Mind:   return 1;
            // 所有的 Time 类型权重都一样
            case E_DiceType.Time1:
            case E_DiceType.Time2:
            case E_DiceType.Time3:
            case E_DiceType.Time4: return 2;
            case E_DiceType.Wild:  return 3;
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
    }
/// <summary>
/// 清空选中骰子
/// </summary>
    public void ClearSelected()
    {
        selectedDice.Clear();
    }
/// <summary>
/// 把某一类骰子的列表按从小到大排序，并初始化或更新索引值index
/// </summary>
/// <param name="type"></param>
    public void SortPoolByValue (E_DiceType type)
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
    }
/// <summary>
/// 不能删
/// </summary>
    private DiceManager()
    {
        
    }
}
