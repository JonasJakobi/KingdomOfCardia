using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all towers in the game. Registers on the grid and can take damage and be destroyed.
/// Specific implementations of towers should inherit from this class.
/// </summary>
public class BaseTower : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int currentLevel = 0;

    [SerializeField] protected TowerUpgrade currentUpgrade;

    [SerializeField] private TowerUpgradePath upgradePath;

    [SerializeField] private bool isNexus = false;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        StartCoroutine(SmallDelay());
    }

    public IEnumerator SmallDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Base Tower placed");
        currentUpgrade = upgradePath.upgrades[currentLevel];
        ApplyUpgrade();

        health = currentUpgrade.health;
        UIChangeManager.Instance.UpdateHP();
        var tileHere = GridManager.Instance.GetTileAtPosition(transform.position);

        tileHere.SetHasBuilding(true, this.gameObject);

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
        UIChangeManager.Instance.UpdateHP();
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Towers))
        {
            Debug.Log("Tower " + this.gameObject.name + ": " + health + "/" + currentUpgrade.health + " health left.");
        }
        if (health <= 0 && isNexus)
        {
            GameManager.Instance.ChangeGameState(GameState.GameOver);
            Debug.Log("Game Over BaseTower!");
            Destroy(this.gameObject);
        }
        if (health <= 0 && !isNexus)
        {
            Destroy(this.gameObject);
        }
    }

    public void ShieldThisBaseTower(int healthAmount, float duration)
    {
        StartCoroutine(GiveTemporaryHP(healthAmount, duration));
    }

    private IEnumerator GiveTemporaryHP(int healthAmount, float duration)
    {
        int currentHealth = health;
        health += healthAmount;
        UIChangeManager.Instance.UpdateHP();
        yield return new WaitForSeconds(duration);
        if (currentHealth <= health)
        {
            health = currentHealth;
        }
        UIChangeManager.Instance.UpdateHP();
    }

    public int GetCurrentHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return currentUpgrade.health;
    }


    public bool GetIsNexus()
    {
        return isNexus;
    }
    /// <summary>
    /// Applies the current upgrade to the tower. Changes the sprite if there is a new one.
    /// </summary>
    private void ApplyUpgrade()
    {
        var maxHealthBefore = currentUpgrade.health;
        currentUpgrade = upgradePath.upgrades[currentLevel];
        var maxHealthDiff = currentUpgrade.health - maxHealthBefore;
        health += maxHealthDiff; //Only 'heal' our tower by the amount of health we gained

        //Change the sprite if there is a new one.
        if (currentUpgrade.upgradeSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = currentUpgrade.upgradeSprite;
        }
        if (GetComponentInChildren<TowerStars>() != null)
        {
            GetComponentInChildren<TowerStars>().SetStars(currentUpgrade.starLevel, currentUpgrade.starColor);
        }
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
    /// <summary>
    /// Upgrades the tower to the next level in the upgrade path. If the tower is already at max level, logs an error.
    /// Applies the upgrade to the tower.
    /// Doesnt check for cost, so make sure to check that before calling this function, as well as reducing money.
    /// </summary>
    public void Upgrade()
    {
        if (currentLevel + 1 < upgradePath.upgrades.Length)
        {
            if (MoneyManager.Instance.CanAfford(upgradePath.upgrades[currentLevel + 1].cost))
            {
                MoneyManager.Instance.RemoveMoney(upgradePath.upgrades[currentLevel + 1].cost);
                currentLevel++;
                ApplyUpgrade();
            }

            else
            {
                Debug.LogError("Tried to upgrade tower " + this.gameObject.name + " but there is not enough money to upgrade it");
            }

        }
        else
        {
            //error log
            Debug.LogError("Tried to upgrade tower " + this.gameObject.name + " but it is already at max level (level " + currentLevel + ")");
        }
    }
}



