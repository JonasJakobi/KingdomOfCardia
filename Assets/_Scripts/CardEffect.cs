using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void ApplyEffect(Enemy enemy);
}

[CreateAssetMenu(fileName = "DamageEffect", menuName = "CardEffect/Damage")]
public class DamageEffect : CardEffect
{
    public int damageAmount;

    public override void ApplyEffect(Enemy enemy)
    {
        enemy.TakeDamage(damageAmount);
    }
}
