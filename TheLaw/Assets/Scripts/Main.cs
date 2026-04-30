using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        // 初始化管理器
        AudioManager.Instance.Init();
        // 显示开始界面
        UIManager.Instance.CreatPanel<StartMenuPanel>(E_UILayer.Middle);
    }
}
