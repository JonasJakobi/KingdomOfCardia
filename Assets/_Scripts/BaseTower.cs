using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
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

    /// <summary>
    /// Can be selected by the player to upgrade/ see stats
    /// </summary>
    public bool isSelectable = false;
    private Tween scaleTween;
    public event System.Action OnTowerDestroyed;
    public Vector3 originalScale;
    public Vector3 selectedScale;
    public bool isSelected = false;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        originalScale = transform.localScale;
        selectedScale = originalScale * 1.5f;
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, 1f).SetEase(Ease.OutBack);
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
        if (!isNexus)
        {
            yield return new WaitForSeconds(0.95f);
            isSelectable = true;
        }


    }
    public TowerUpgrade GetTowerUpgrade(bool next = false)
    {
        if (next && upgradePath.upgrades.Length > currentLevel + 1)
        {
            return upgradePath.upgrades[currentLevel + 1];
        }
        return currentUpgrade;
    }
    public void DestroyTower()
    {
        if (!isNexus)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (GridManager.Instance != null)
        {
            var tileHere = GridManager.Instance.GetTileAtPosition(transform.position);
            tileHere.SetHasBuilding(false, this.gameObject);
        }
        var placers = FindObjectsOfType<TowerPlacerUI>();

        OnTowerDestroyed?.Invoke();

    }
    private void RefundMoney()
    {
        for (int i = 0; i < currentLevel; i++)
        {
            MoneyManager.Instance.AddMoney((int)(upgradePath.upgrades[i].cost / 2));
        }
    }
    public void TakeDamage(int damage)
    {
        int trueDamage = Mathf.RoundToInt(damage * Constants.Instance.damageToTowersMultiplier);
        health -= trueDamage;
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
        if (currentLevel > 0) // play animation on every upgrade
        {
            UpgradeAnimation();
        }

    }
    private void UpgradeAnimation()
    {
        if (scaleTween != null)
        {
            scaleTween.Kill();
            if (isSelected)
            {
                transform.localScale = selectedScale;
            }
            else
            {
                transform.localScale = originalScale;

            }
        }
        //grow and then shrink as feedback with DoScale
        scaleTween = transform.DOScale(transform.localScale * 1.2f, 0.25f).SetEase(Ease.InSine).OnComplete(() =>
        {
            transform.DOScale(transform.localScale / 1.2f, 0.25f).SetEase(Ease.InSine);
        });
        AudioSystem.Instance.PlayAnvilSound(currentLevel);
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
                //flash money red
                MoneyManager.Instance.FlashMoneyText();
            }

        }
        else
        {
            //error log
            Debug.LogError("Tried to upgrade tower " + this.gameObject.name + " but it is already at max level (level " + currentLevel + ")");
        }
    }
    public void SelectTower()
    {
        scaleTween.Kill();
        isSelected = true;
        scaleTween = transform.DOScale(selectedScale, 0.3f).SetEase(Ease.InSine);
    }
    public void DeSelectTower()
    {
        scaleTween.Kill();
        isSelected = false;
        scaleTween = transform.DOScale(originalScale, 0.3f).SetEase(Ease.InSine);
    }
}



