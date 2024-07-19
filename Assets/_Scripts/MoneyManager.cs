using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : Singleton<MoneyManager>
{
    [Header("Start Money is now in Constants")]
    public long money = 0;

    public long moneyGained = 0;

    public int moneySpent = 0;

    [SerializeField]
    private TMP_Text moneyText;

    void Start()
    {
        money = Constants.Instance.startMoney;
        UIChangeManager.Instance.UpdateMoney();
    }

    public void AddMoney(long amount)
    {
        money += amount;
        moneyGained += amount;
        UIChangeManager.Instance.UpdateMoney();
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Enemies))
            Debug.Log("Added " + amount + " money. Total: " + money + " money.");
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
        moneySpent += amount;
        UIChangeManager.Instance.UpdateMoney();
        Debug.Log("Removed " + amount + " money. Total: " + money + " money.");
    }

    public bool CanAfford(int amount)
    {

        return money >= amount;
    }

    private IEnumerator FlashMoney()
    {
        moneyText.color = Color.red;
        yield return new WaitForSeconds(1f);
        moneyText.color = Color.white;
    }
    public void FlashMoneyText()
    {
        StopAllCoroutines();
        StartCoroutine(FlashMoney());
    }
}
