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

    public GameObject WaveAlertPrefab;

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

    //Create GameObject to indicate the SpawnPoints in the following round
    //param spawnPoint = The spawnPoint Object to indicate
    public void createWaveAlert(SpawnPoint spawnPoint)
    {
        Vector3 randomPos = new Vector3(spawnPoint.widthPosition, spawnPoint.heightPosition, 0);
        GameObject newWaveAlert = Instantiate(WaveAlertPrefab, randomPos, Quaternion.identity);
    }

    //Remove the SpawnPoint indicators
    public void removeAllWaveAlerts()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "WaveAlert(Clone)")
            {
                Destroy(obj);
            }

        }
    }
}
