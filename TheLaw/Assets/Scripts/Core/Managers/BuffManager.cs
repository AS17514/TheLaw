using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BuffManager :ManagerBase<BuffManager>
{
    public BuffManager()
    {
        
    }

    protected override void Init()
    {
        base.Init();
        
    }

    public Entity entity;
    public Player player;
    public void Register(CharacterBase buffEntity)
    {
        // 根据身份存入对应的位置
        if (buffEntity.IsPlayer) 
            player = (buffEntity as Player);
        else 
            entity = (buffEntity as Entity);
    }
    public void Unregister(Entity buffEntity)
    {
        if (buffEntity.IsPlayer)
        {
            if (player == buffEntity) player = null; // 清空玩家引用
        }
        else
        {
            if (entity == buffEntity) entity = null; // 清空怪物引用
        }
    }
    
}
