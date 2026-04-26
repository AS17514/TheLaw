using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerMonoBase<T> : MonoBehaviour where T : MonoBehaviour
{
    // 继承了mono，自动处理挂载的单例基类
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject { name = typeof(T).Name };
                instance = gameObject.AddComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
    }
}

