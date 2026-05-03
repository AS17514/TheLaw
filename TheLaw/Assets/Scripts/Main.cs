using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//呼呼，伊利哇啦——
public class Main : MonoBehaviour
{
    void Awake()
    {
        GameInitializer.Init();
        // _ = EventCenter.Instance;
        // _ = EventManager.Instance;
        // _ = StateManager.Instance;
        // _ = BuffManager.Instance;
        // _ = DiceManager.Instance;
        // _ = ProgressManager.Instance;
        // SkillManager.InitSkills();
    }
    void Start()
    {
        // 初始化管理器
        AudioManager.Instance.Init();
        // 显示开始界面
        UIManager.Instance.CreatPanel<StartMenuPanel>(E_UILayer.Middle);
    }
}
