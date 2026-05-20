using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part1_2 : Part
{
    public void InitPart()
    {
        partName = "盘子";
        id = 2;
        maxHp = 5;
        hp = maxHp;
    }

    public void Awake()
    {
        InitPart();
    }
    public override void Die()
    {
        ProgressManager.Instance.PartStateChange(1,false);
        base.Die();
        if (ProgressManager.Instance.nowEntities[1] != null && ProgressManager.Instance.nowEntities[1] is Part1_1 part1)
        {
            if (part1.isDestroyed)
            {
                StateManager.Instance.ChangeState(E_StateType_1.exhausted);
            }
        }
    }
    /// <summary>
    /// 静态方法，外部需要生成部位的时候调用。
    /// </summary>
    public static void PartApear()=> Part.SpawnPart<Part1_2>(2, "Part1_2");
}
