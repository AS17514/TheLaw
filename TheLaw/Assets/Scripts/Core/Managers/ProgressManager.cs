using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : ManagerBase<ProgressManager>
{
    public int level ;//当前关卡
    public int phase;//当前时间段（当前回合数)
    public int initialTimeProgress;//本关初始时间进度上限
    public int currentTimeProgress;//本关当前时间进度上限
    public int timeProgress;//当前时间进度
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
    }
/// <summary>
/// 时间段（当前回合数）加一
/// </summary>
    public void AddPhase()
    {
        ++this.phase;
    }
}
