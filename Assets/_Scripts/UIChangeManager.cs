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
    public TMP_Text Stat1;
    public TMP_Text Stat2;
    public TMP_Text Stat3;
    public TMP_Text Stat4;
    public TMP_Text Stat5;
    public TMP_Text Stat6;
    public TMP_Text Stat7;

    public GameObject WaveAlertPrefab;

    public GameObject towerPlaceUI;

    public GameObject gameOverUI;

    public GameObject tutorialUI;

    public bool tutorialDisabledPermanently = false;
    public GameObject tutorialParent;
    public Tutorial tutorialScript;

    public int damageDealt = 0;


    // Start is called before the first frame update
    void Start()
    {
        updateRound();
        startTutorial();
    }

    public void updateRound()
    {
        RoundText.text = "Runde " + RoundManager.Instance.round.ToString();
    }

    public void updateMoney()
    {
        MoneyText.text = MoneyManager.Instance.money.ToString() + " Gold";
    }

    //Update percentage of HP (rounded to 2 decimal points)
    public void updateHP()
    {
        BaseTower baseTower = GameObject.FindWithTag("Goal").GetComponent<BaseTower>();
        float percentageHealth = Mathf.Round((((float)baseTower.GetCurrentHealth() / (float)baseTower.GetMaxHealth()) * 100.0f) * 100f) / 100f;
        HPText.text = percentageHealth.ToString() + " % HP";
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

    public void showBuildModeUI()
    {
        towerPlaceUI.SetActive(true);

    }

    public void removeBuildModeUI()
    {
        towerPlaceUI.SetActive(false);
        removeAllWaveAlerts();
    }

    public void startTutorial()
    {
        if (tutorialDisabledPermanently != true) tutorialUI.SetActive(true);
    }

    public void TutorialCheck()
    {
        if (tutorialScript.tutorialSkipped == false && tutorialDisabledPermanently == false)
        {
            tutorialParent.SetActive(true);
        }
    }

    public void showGameOver()
    {
        gameOverUI.SetActive(true);
        Stat1.text = RoundManager.Instance.round.ToString();
        Stat2.text = RoundManager.Instance.enemiesDefeated.ToString();
        Stat3.text = damageDealt.ToString();
        Stat4.text = 0.ToString(); //Noch nicht implementiert
        Stat5.text = MoneyManager.Instance.moneyGained.ToString();
        Stat6.text = MoneyManager.Instance.moneySpent.ToString();
        Stat7.text = CardManager.Instance.cardsPlayed.ToString();
    }
}
