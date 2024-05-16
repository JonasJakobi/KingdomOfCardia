using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    public int money = 0;

    public void AddMoney(int amount)
    {
        money += amount;
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Enemies))
            Debug.Log("Added " + amount + " money. Total: " + money + " money.");
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
    }

    public bool CanAfford(int amount)
    {
        return money >= amount;
    }
}
