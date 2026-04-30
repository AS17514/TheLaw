using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : ManagerMonoBase<ProgressManager>
{
    public int level;//当前关卡
    public int phase;//当前时间段（当前回合数)
    public int initialTimeProgress;//本关初始时间进度上限
    public int currentTimeProgress;//本关当前时间进度上限
    public int timeProgress;//当前时间进度
    public int maxPhaseDice;//当前时间段骰子消耗上限数量
    public int phaseDice;//当前时间段骰子数量，用完了就结束当前时间段。
    public CharacterBase[] nowEntities = new CharacterBase[4];
    public Player player;
    public ProgressManager()
    {
    }
    /// <summary>
    /// 初始化当前进度，设置当前关卡与本关初始时间进度上限
    /// </summary>
    /// <param name="initialTimeProgress">初始时间进度上限</param>
    /// <param name="currentTimeProgress">本关当前时间进度上限</param>
    /// <param name="phaseDice">当前时间段骰子数量</param>
    public void Init(int initialTimeProgress, int currentTimeProgress, int phaseDice)
    {
        this.initialTimeProgress = initialTimeProgress;
        this.currentTimeProgress = currentTimeProgress;
        maxPhaseDice = phaseDice;
        this.phaseDice = phaseDice;
    }
    /// <summary>
    /// 设置当前时间进度上限
    /// </summary>
    /// <param name="currentTimeProgress"></param>
    public void SetCurrentTimeProgress(int currentTimeProgress)
    {
        this.currentTimeProgress = currentTimeProgress;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_MaxTimeProgress, this.currentTimeProgress);
    }
    /// <summary>
    /// 消耗时间骰子的时候调用，推进时间段的进行
    /// </summary>
    /// <param name="timeValue"></param>
    public void AdvancePhase(int timeMount)
    {
        phaseDice = Math.Clamp(phaseDice - timeMount, 0, maxPhaseDice);
        if (phaseDice == 0)
        {
            AddPhase();
        }
    }
    /// <summary>
    /// 时间段（当前回合数）加一
    /// </summary>
    public void AddPhase()
    {
        ++this.phase;
        phaseDice = maxPhaseDice;
        StateManager.Instance.ExecuteCurrentDesire();
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Phase, this.phase);
        //更新许愿为可用状态
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);
        if (level == 1)
        {
            if (nowEntities[0] is Entity1 entity1)
            {
                if (entity1.isDesire_UrgentUse == true)
                {
                    entity1.isDesire_UrgentUse = false;
                    SetCurrentTimeProgress(initialTimeProgress);
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_MaxTimeProgress, initialTimeProgress);
                }
            }
            else
            {
                Debug.Log("nowEntities[0] 不是 Entity1，吱吱吱吱，这不应该啊。。。。不好，我的代码。。。");
            }
        }
    }
    /// <summary>
    /// 改变时间进度
    /// </summary>
    /// <param name="add"></param>
    public void AddTimeProgress(int add)
    {
        timeProgress = Math.Clamp(timeProgress + add, 0, this.currentTimeProgress);
        if (timeProgress == currentTimeProgress)
        {
            StateManager.Instance.ExecuteCurrentAction();
            timeProgress = currentTimeProgress;
        }
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeProgress, this.timeProgress);

    }

    #region IntoNewLevel
    public void intoNewLevel(int level)
    {
        if (level <= 4 && level > 0)
        {
            this.level = level;
            switch (level)
            {
                case 1:
                    initLevel1();
                    break;
                case 2:
                    initLevel2();
                    break;
                case 3:
                    initLevel3();
                    break;
                case 4:
                    initLevel4();
                    break;

            }
        }
    }

    public void AddPlayer()
    {
        if (player == null)
        {
            GameObject managerObj = new GameObject("Player");
            player = managerObj.AddComponent<Player>();
        }
    }

    public void initLevel1()
    {
        AddPlayer();
        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity1");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        nowEntities[0] = managerObj.AddComponent<Entity1>();

        Init(5, 5, 4);
    }
    public void initLevel2()
    {
        AddPlayer();
        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity2");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        nowEntities[0] = managerObj.AddComponent<Entity2>();

        //加载当前关卡已解锁的许愿，并且把许愿更新为可用状态。
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Wish1ToUnlocked);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);

        Init(5, 5, 4);
    }
    public void initLevel3()
    {
        AddPlayer();
        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity3");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        nowEntities[0] = managerObj.AddComponent<Entity3>();

        //加载当前关卡已解锁的许愿，并且把许愿更新为可用状态。
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Wish1ToUnlocked);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Wish2ToUnlocked);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);

        Init(6, 6, 1);
    }
    public void initLevel4()
    {
        AddPlayer();
        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity4");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        nowEntities[0] = managerObj.AddComponent<Entity4>();

        //加载当前关卡已解锁的许愿，并且把许愿更新为可用状态。
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Wish1ToUnlocked);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Wish2ToUnlocked);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Wish3ToUnlocked);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);

    }
    #endregion
}
