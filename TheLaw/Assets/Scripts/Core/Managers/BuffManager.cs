using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BuffManager :ManagerBase<BuffManager>
{
    public  BuffManager()
    {}
    public Dictionary<Character,int> buffPoll=new Dictionary<Character,int>();
    public Entity entity;
    public Player player;
    
    
}
