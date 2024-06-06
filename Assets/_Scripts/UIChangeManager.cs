using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIChangeManager : Singleton<UIChangeManager>
{

    public TMP_Text RoundText;

    public TMP_Text MoneyText;

    public TMP_Text HPText;

    // Start is called before the first frame update
    void Start()
    {
        updateRound();
    }

    public void updateRound()
    {
        RoundText.text = "Runde " + RoundManager.Instance.round.ToString();
    }

    public void updateMoney()
    {
        MoneyText.text = MoneyManager.Instance.money.ToString() + " Gold";
    }

    //Klappt noch nicht: CurrentHP und MaxHP sind immer 100.
    public void updateHP()
    {
        BaseTower baseTower = FindObjectOfType<BaseTower>();
        float percentageHealth = ((baseTower.GetCurrentHealth() / baseTower.GetMaxHealth()) * 100);
        HPText.text = percentageHealth.ToString() + " % HP";
        Debug.Log("CurrentHealth: " + baseTower.GetCurrentHealth() + ", MaxHealth: " + baseTower.GetMaxHealth());
        Debug.Log("Kann ich %? Das sind " + percentageHealth + "%!");
    }
}
