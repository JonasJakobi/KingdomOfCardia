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

    public void DealDamage(int targetNumber)
    {
        int counter = 0;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            counter++;
            if (counter >= targetNumber)
            {
                enemy.TakeDamage(enemy.maxHealth * 10000); // cardData.damageAmount sollte den Schaden der Karte enthalten
                counter = 0;

            }

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
            int healFor = (missingHealth / 100) * healAmount;
            baseTower.HealBaseTower(healFor);
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
            enemy.SlowEnemy(slow, duration, true);
        }
        AudioSystem.Instance.ChangePitch(0.4f, duration);
    }


    //Increase player gold by amount in percent
    public void MultiplyGold(int amount)
    {
        long moneyGain = MoneyManager.Instance.money * amount / 100;
        Debug.Log("So viel Geld: " + moneyGain);
        MoneyManager.Instance.AddMoney(moneyGain);
    }

    // Starts Fire Damage
    public void StartFireDamage(float duration, int damage)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.StartFireDamage(duration, damage);
        }
    }

    //Starts Necrotic Damage
    public void StartNecroticDamage(float duration, int damage)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.StartNecroticDamage(duration, damage);
        }
    }

    //Starts Electricity Damage

    public void StartElectricDamage(float duration, int damage)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.StartElectricDamage(duration, damage);
        }
    }

    //reduces damage that enemies do
    public void ReduceDamage(float reduce, float duration)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.ReduceDamage(reduce, duration);
        }
    }

    //Movement and AttackSpeed is Slowed
    public void OverallSlower(float slow, float duration)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.SlowMovementAndAttack(slow, duration);
        }
        AudioSystem.Instance.ChangePitch(0.4f, duration);
    }


}
