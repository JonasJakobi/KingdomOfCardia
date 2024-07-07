using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField]
    private int startingMoney = 1;
    public int money = 0;

    public int moneyGained = 0;

    public int moneySpent = 0;

    void Start()
    {
        money = startingMoney;
        UIChangeManager.Instance.UpdateMoney();
    }

    public void AddMoney(int amount)
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
}
