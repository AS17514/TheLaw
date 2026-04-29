using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_DiceType
{
    Action,
    Mind,
    Wild,
    Time1,
    Time2,
    Time3,
    Time4,
    // 【新增】这是一个虚拟类型，池子里永远不会有这种骰子，仅用于技能条件判断
    TimeAny,
    // 【新增】这是一个虚拟类型，池子里永远不会有这种骰子，仅用于技能条件判断
    ActionOrMind,
    // 【新增】这是一个特殊类型，玩家池子里永远不会有这种骰子，仅用于怪物池
    EntityDice,
}
