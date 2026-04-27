using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : CharacterBase
{
    public List<Part> parts=new List<Part>();
    public int initialDesire;
    public string name;
    public override void Die()
    {
        
    }
}
