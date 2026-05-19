using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_TutorialType
{
    None,
    MeetDodge,       // 第一次遇到闪避选项（怪物骰子池介绍）level1
    PartAppear,      // 第一次出现怪物部位level1
    FirstCommunicate,// 第一次出现交流选项
    StateSwitch,     // 第一次状态切换
    PlayerBuff,      // 第一次玩家获得buff level2
    MeetManyDodge,   // 第一次遇到多个闪避选项  level3
    
}
