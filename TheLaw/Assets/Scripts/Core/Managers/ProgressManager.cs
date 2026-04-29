using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : ManagerMonoBase<ProgressManager>
{
    public int level ;//当前关卡
    public int phase;//当前时间段（当前回合数)
    public int initialTimeProgress;//本关初始时间进度上限
    public int currentTimeProgress;//本关当前时间进度上限
    public int timeProgress;//当前时间进度
    public Entity nowEntitie;
    public ProgressManager()
    {
    }
/// <summary>
/// 初始化当前进度，设置当前关卡与本关初始时间进度上限
/// </summary>
/// <param name="initialTimeProgress"></param>
/// <param name="currentTimeProgress"></param>
    public void Init(int initialTimeProgress,int currentTimeProgress)
    {
    this.initialTimeProgress = initialTimeProgress;
    this.currentTimeProgress = currentTimeProgress;
    }
/// <summary>
/// 设置当前时间进度上限
/// </summary>
/// <param name="currentTimeProgress"></param>
    public void SetCurrentTimeProgress(int currentTimeProgress)
    {
        this.currentTimeProgress = currentTimeProgress;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_MaxTimeProgress,this.currentTimeProgress);
    }
/// <summary>
/// 时间段（当前回合数）加一
/// </summary>
    public void AddPhase()
    {
        ++this.phase;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Phase,this.phase);
    }
/// <summary>
/// 改变时间进度
/// </summary>
/// <param name="add"></param>
    public void AddTimeProgress(int add)
    {
        timeProgress=Math.Clamp(timeProgress+add,0,this.currentTimeProgress);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeProgress,this.timeProgress);
    }

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

    public void initLevel1()
    {
        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity1");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        nowEntitie = managerObj.AddComponent<Entity1>();
    }
    public void initLevel2()
    {
        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity2");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        nowEntitie  = managerObj.AddComponent<Entity2>();

    }
    public void initLevel3()
    {
        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity3");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        nowEntitie = managerObj.AddComponent<Entity3>();

    }
    public void initLevel4()
    {
        // 1. 创建一个新的空物体
        GameObject managerObj = new GameObject("Entity4");

        // 2. 动态添加脚本，并获取引用
        // 注意：AddComponent 会自动返回该脚本的实例
        nowEntitie = managerObj.AddComponent<Entity4>();

    }
}
