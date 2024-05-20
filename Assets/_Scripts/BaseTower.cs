using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all towers in the game. Registers on the grid and can take damage and be destroyed.
/// Specific implementations of towers should inherit from this class.
/// </summary>
public class BaseTower : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health = 100;

    [Tooltip("The range in which the tower can detect enemies in grid squares")]
    protected int range = 5;
    [Tooltip("The delay between attacks in seconds")]
    [SerializeField] protected float attackSpeed = 0.2f;
    [SerializeField] protected int damage = 15;

    [SerializeField] private int currentLevel = 0;

    [SerializeField] private TowerUpgrade currentUpgrade;
    [SerializeField] private TowerUpgradePath upgradePath;

    [SerializeField] private bool isNexus = false;
    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;
        var tileHere = GridManager.Instance.GetTileAtPosition(transform.position);
        tileHere.SetHasBuilding(true, this.gameObject);
        ApplyUpgrade();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        var tileHere = GridManager.Instance.GetTileAtPosition(transform.position);
        tileHere.SetHasBuilding(false, this.gameObject);

    }

    public void TakeDamage(int damage)
    {

        health -= damage;
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Towers))
        {
            Debug.Log("Tower " + this.gameObject.name + ": " + health + "/" + maxHealth + " health left.");
        }
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public int GetCurrentHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }


    public bool GetIsNexus()
    {
        return isNexus;
    }

    private void ApplyUpgrade()
    {
        currentUpgrade = upgradePath.upgrades[currentLevel];
        var maxHealthDiff = currentUpgrade.health - maxHealth;
        maxHealth = currentUpgrade.health;
        health += maxHealthDiff; //Only 'heal' our tower by the amount of health we gained
        damage = (int)currentUpgrade.damage;
        range = currentUpgrade.range;
        attackSpeed = currentUpgrade.attackSpeed;


    }
    /// <summary>
    /// Returns the price of the next upgrade in the upgrade path. Returns -1 if we are at max level.
    /// </summary>
    /// <returns>The price of next upgrade or -1 if no further upgrade is possible.</returns>
    public float GetCostOfUpgrading()
    {
        if (currentLevel + 1 < upgradePath.upgrades.Length)
        {
            return upgradePath.upgrades[currentLevel + 1].cost;
        }
        else
        {
            return -1; //Return -1 if we are at max level, so we can check for it
        }
    }
    public void Upgrade()
    {
        if (currentLevel + 1 < upgradePath.upgrades.Length)
        {
            currentLevel++;
            ApplyUpgrade();
        }
        else
        {
            //error log
            Debug.LogError("Tried to upgrade tower " + this.gameObject.name + " but it is already at max level (level " + currentLevel + ")");
        }
    }
}



