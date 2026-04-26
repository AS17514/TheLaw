using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    public int hp;
    public int maxHp;
    public abstract void Die();

    public virtual void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            Debug.Log(gameObject.name + " damaged " + damage+"，老大，伤害应大于0喵");
            return;
        }
        hp = Mathf.Clamp(hp-damage, 0, maxHp);
        if (hp <= 0)
        {
            Die();
        }
    }

    public virtual void BeAttacked(int atk)
    {
        TakeDamage( atk);
    }
}
