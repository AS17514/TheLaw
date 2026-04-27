using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ManagerBase<T> where T:class
{
    private static T instance;

    //属性的方式
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Activator.CreateInstance(typeof(T), true) as T;
                (instance as ManagerBase<T>)?.Init();
            }
            return instance;
        }
    }
/// <summary>
/// 用于供子类执行一些在脚本刚生成就执行的逻辑,有需要就重写这个方法
/// </summary>
    protected virtual void Init()
    {
        
    }
}