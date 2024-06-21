using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField]
    private int startingMoney = 1;
    public int money = 0;

    void Start()
    {
        money = startingMoney;
        UIChangeManager.Instance.updateMoney();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UIChangeManager.Instance.updateMoney();
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Enemies))
            Debug.Log("Added " + amount + " money. Total: " + money + " money.");
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
        UIChangeManager.Instance.updateMoney();
        Debug.Log("Removed " + amount + " money. Total: " + money + " money.");
    }

    public bool CanAfford(int amount)
    {
       
        return money >= amount;
    }
}
