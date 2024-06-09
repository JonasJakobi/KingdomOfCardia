using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Air,
    Blunt,
    Earth,
    Electricity,
    Fire,
    Holy,
    Ice,
    Light,
    Water,
    Necrotic,
    Piercing,
    None
}

[CreateAssetMenu(fileName = "New CardEffect", menuName = "CardEffect/General")]
public class CardEffect : ScriptableObject
{
    public DamageType damageType;
    public int multiplier = 1;

    public void DealDamage(int damage)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(damage * multiplier); // cardData.damageAmount sollte den Schaden der Karte enthalten
        }
    }

    /// <summary>
    /// Heal the base towers for percentage of its missing health
    /// </summary>
    /// <param name="healAmount">The amount of missing health restored</param>
    public void HealBaseTowers(int healAmount)
    {
        BaseTower[] baseTowers = FindObjectsOfType<BaseTower>();
        foreach (BaseTower baseTower in baseTowers)
        {
            int missingHealth = baseTower.GetMaxHealth() - baseTower.GetCurrentHealth();
            int healFor = ((missingHealth / 100) * healAmount) * -1;
            baseTower.TakeDamage(healFor);
            Debug.Log("Healed base towers for " + healAmount + "% of their missing health!");
        }
    }

    //Only looks at the first loaded BaseTower
    public void ShieldBaseTowers(int shieldStrength, float duration)
    {
        BaseTower baseTower = GameObject.FindWithTag("Goal").GetComponent<BaseTower>();
        baseTower.ShieldThisBaseTower(shieldStrength, duration);
    }

    public void SlowAllEnemies(int slow, float duration)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.SlowEnemy(slow, duration);
        }
    }
}
