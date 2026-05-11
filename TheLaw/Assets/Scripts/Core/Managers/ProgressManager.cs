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
    public CharacterBase[] nowEntities = new CharacterBase[5];
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
        timeProgress = 0;
        phase = 0;
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
        if (timeProgress > currentTimeProgress)
        {
            timeProgress = currentTimeProgress;
            AddTimeProgress(0);
        }
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_MaxTimeProgress, this.currentTimeProgress);
    }

    public void SetmaxPhaseDice(int maxPhaseDice)
    {
        this.maxPhaseDice = maxPhaseDice;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_MaxTimeProgress, this.maxPhaseDice);
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
        if (level == 5)
        {
            if (nowEntities[0] is Entity5 entity5)
            {
                player.RemoveLevel5Buff();
                bool isStzteChange = false;
                switch (entity5.playerState1)
                {
                    case 1:
                        player.AddBuff(E_BuffType.Up, 1);
                        isStzteChange = true;
                        break;
                    case 0:
                        player.AddBuff(E_BuffType.Down, 1);
                        isStzteChange = true;
                        break;
                }

                switch (entity5.playerState2)
                {
                    case 1:
                        player.AddBuff(E_BuffType.Left, 1);
                        isStzteChange = true;
                        break;
                    case 0:
                        player.AddBuff(E_BuffType.Right, 1);
                        isStzteChange = true;
                        break;
                }

                if (isStzteChange)
                {
                    StateManager.Instance.ChangeState(E_StateType_5.unbalance);
                }
                if (entity5.IsPlayerFree)
                {
                    entity5.IsPlayerFree = false;
                    DiceManager.Instance.AddTimeDice(1);
                }
            }
        }
        ++this.phase;
        phaseDice = maxPhaseDice;
        int i = 0;
        if (player.GetBuff(E_BuffType.Up) > 0)
            i = 1;
        DiceManager.Instance.AddTimeDice(maxPhaseDice + i);

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

        StateManager.Instance.ExecuteCurrentDesire();
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Phase);

        //更新许愿为可用状态
        if (level == 3)
        {
            if (StateManager.Instance.currentState is E_StateType_3.normal)
            {
                List<EntityDice> tempList = new List<EntityDice>();
                var pool = DiceManager.Instance.entityDicePool;
                for (int im = pool.Count - 1; im >= 0; im--)
                {
                    EntityDice entityDice = pool[im];
                    if (!entityDice.isValid)
                    {
                        entityDice.isValid = true;
                    }
                    else
                    {
                        pool.RemoveAt(im);
                    }
                }
                DiceManager.Instance.SortEntityPoolByValue();
                //DiceManager.Instance.ClearEntityPool();
            }
            if (nowEntities[0] is Entity3 entity3)
            {
                if (entity3.IsRumorsAboutDisinterestUse)
                {
                    entity3.IsRumorsAboutDisinterestUse = false;
                    return;
                }
            }
        }
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);
    }
    /// <summary>
    /// 改变时间进度
    /// </summary>
    /// <param name="add"></param>
    public void AddTimeProgress(int add)
    {
        int i = 0;
        if (player.GetBuff(E_BuffType.Right) > 0)//玩家消耗时间骰时时间进度额外-1
            i = 1;
        timeProgress = Math.Clamp(timeProgress + add - i, 0, this.currentTimeProgress);
        if (timeProgress == currentTimeProgress)
        {
            StateManager.Instance.ExecuteCurrentAction();
            timeProgress = 0;
        }
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeProgress);

        if (StateManager.Instance.currentState is E_StateType_3.weightless)
        {
            DiceManager.Instance.ClearEntityPool();
        }
    }

    #region IntoNewLevel
    public void intoNewLevel(int level)
    {
        if (level <= 5 && level > 0)
        {
            // 在进入新关卡、生成新怪物之前，先彻底清理上一关的残留数据
            ClearOldEntities();
            // 清空上一关遗留的骰子数据
            DiceManager.Instance.ClearPool();       // 清空基础骰子池、怪物骰子池并刷新对应UI
            DiceManager.Instance.ClearSelected();   // 清空选中区的骰子并刷新对应UI

            EventCenter.Instance.ClearEventListeners(E_EventType.Logic_PlayerActionExecuted);
            
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
                case 5:
                    initLevel5();
                    break;
            }
        }
    }

    public void AddPlayer()
    {
        if (player == null)
        {
            GameObject managerObj = new GameObject("Player");
            Player newplayer = managerObj.AddComponent<Player>();
            player = newplayer;
            player.InitPlayer();
        }
        else
        {
            player.InitPlayer();//重新初始化玩家,回血，重置欲望之类的。
        }
    }
    /// <summary>
    /// 清理上一关残留的所有实体数据，并销毁对应的游戏物体
    /// </summary>
    public void ClearOldEntities()
    {
        for (int i = 0; i < nowEntities.Length; i++)
        {
            if (nowEntities[i] != null)
            {
                // 1. 销毁实体挂载的整个 GameObject。
                // 这一步非常关键！它会将物体从场景中彻底移除，并且触发该物体上所有脚本的 OnDestroy 方法。
                Destroy(nowEntities[i].gameObject);

                // 2. 将数组里的引用置空，防止其他脚本拿到了已经被销毁的物体的引用（报 MissingReferenceException）
                nowEntities[i] = null;
            }
        }
    }
    public void initLevel1()
    {
        AddPlayer();

        EventManager.Instance.RegisterOptions(1);

        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity1");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        Entity1 entity1 = managerObj.AddComponent<Entity1>();
        nowEntities[0] = entity1;

        entity1.ManualInit();

        Init(5, 5, 4);

        DiceManager.Instance.AddTimeDice(4);
        Debug.Log("initLevel1执行1次，AddTimeDice执行1次");

    }
    public void initLevel2()
    {
        AddPlayer();

        EventManager.Instance.RegisterOptions(2);

        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity2");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        Entity2 entity2 = managerObj.AddComponent<Entity2>();
        nowEntities[0] = entity2;

        entity2.ManualInit();
        //加载当前关卡已解锁的许愿，并且把许愿更新为可用状态。
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);

        Init(4, 4, 5);

        DiceManager.Instance.AddTimeDice(5);
    }
    public void initLevel3()
    {
        AddPlayer();

        player.AddBuff(E_BuffType.Desire, 6);

        EventManager.Instance.RegisterOptions(3);

        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity3");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        Entity3 entity3 = managerObj.AddComponent<Entity3>();
        nowEntities[0] = entity3;

        entity3.ManualInit();
        //加载当前关卡已解锁的许愿，并且把许愿更新为可用状态。
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);

        Init(6, 6, 1);

        DiceManager.Instance.AddTimeDice(1);
    }
    public void initLevel4()
    {

        AddPlayer();

        player.AddBuff(E_BuffType.Desire, 6);

        EventManager.Instance.RegisterOptions(4);

        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity4");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        Entity4 entity4 = managerObj.AddComponent<Entity4>();
        nowEntities[0] = entity4;

        entity4.ManualInit();
        //加载当前关卡已解锁的许愿，并且把许愿更新为可用状态。
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);

        Init(6, 6, 1);

        DiceManager.Instance.AddTimeDice(1);

    }
    public void initLevel5()
    {
        AddPlayer();

        player.AddBuff(E_BuffType.Desire, 0);

        EventManager.Instance.RegisterOptions(5);

        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity5");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        Entity5 entity5 = managerObj.AddComponent<Entity5>();
        nowEntities[0] = entity5;

        entity5.ManualInit();
        //加载当前关卡已解锁的许愿，并且把许愿更新为可用状态。
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToAvailable);

        Init(1, 1, 1);

        DiceManager.Instance.AddTimeDice(1);
    }
    #endregion
}
