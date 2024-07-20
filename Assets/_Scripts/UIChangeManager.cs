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
    public TMP_Text CardUpgradeCost;
    public TMP_Text Stat1;
    public TMP_Text Stat2;
    public TMP_Text Stat3;
    public TMP_Text Stat4;
    public TMP_Text Stat5;
    public TMP_Text Stat6;
    public TMP_Text Stat7;

    public GameObject WaveAlertPrefab;
    public GameObject shieldPrefab;

    public GameObject towerPlaceUI;

    public GameObject gameOverUI;

    public GameObject tutorialUI;

    public GameObject upgradeUI;

    public static bool tutorialDisabledPermanently = false;
    public bool warningQueued = false;
    public GameObject tutorialParent;
    public Tutorial tutorialScript;
    public GameObject speedButtons;
    public GameObject startRoundButton;

    public int damageDealt = 0;

    public int towersPlaced = 0;


    // Start is called before the first frame update
    void Start()
    {
        UpdateRound();
        StartTutorial();
    }

    public void UpdateRound()
    {
        RoundText.text = "Runde " + RoundManager.Instance.round.ToString();
    }

    public void UpdateMoney()
    {
        MoneyText.text = MoneyManager.Instance.money.ToString() + " Gold";
    }

    //Update percentage of HP (rounded to 2 decimal points)
    public void UpdateHP()
    {
        BaseTower baseTower = GameObject.FindWithTag("Goal").GetComponent<BaseTower>();
        float percentageHealth = Mathf.Round((((float)baseTower.GetCurrentHealth() / (float)baseTower.GetMaxHealth()) * 100.0f) * 100f) / 100f;
        if (percentageHealth >= 0f) HPText.text = percentageHealth.ToString() + " % HP";
        else HPText.text = "0% HP";
    }

    public void CreateShield()
    {
        GameObject newShield = Instantiate(shieldPrefab, new Vector3(15, 8, 0), Quaternion.identity);
    }

    public void RemoveShield()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "shield(Clone)")
            {
                AudioSystem.Instance.PlayPloppSound();
                Destroy(obj);
            }

        }
    }

    //Create GameObject to indicate the SpawnPoints in the following round
    //param spawnPoint = The spawnPoint Object to indicate
    public void CreateWaveAlert(SpawnPoint spawnPoint)
    {
        Vector3 randomPos = new Vector3(spawnPoint.widthPosition, spawnPoint.heightPosition, 0);
        GameObject newWaveAlert = Instantiate(WaveAlertPrefab, randomPos, Quaternion.identity);
    }

    //Remove the SpawnPoint indicators
    public void RemoveAllWaveAlerts()
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

    public void ShowBuildModeUI()
    {
        towerPlaceUI.SetActive(true);
        startRoundButton.SetActive(true);
        speedButtons.SetActive(false);
    }

    public void RemoveBuildModeUI()
    {
        startRoundButton.SetActive(false);
        towerPlaceUI.SetActive(false);
        speedButtons.SetActive(true);
        RemoveAllWaveAlerts();
    }

    public void StartTutorial()
    {
        if (tutorialDisabledPermanently != true) tutorialUI.SetActive(true);
    }

    public void TutorialCheck()
    {
        if (tutorialScript.tutorialSkipped == false && tutorialDisabledPermanently == false)
        {
            tutorialParent.SetActive(true);
            tutorialScript.ReactivateTutorial();
        }
    }

    public void QueueWarning()
    {
        if (tutorialScript.warningShowed == false && warningQueued == false)
        {
            warningQueued = true;
        }
    }
    public void WarningCheck()
    {
        if (tutorialScript.warningShowed == false && warningQueued == true)
        {
            tutorialParent.SetActive(true);
            tutorialScript.ShowWarning();
        }
    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        Stat1.text = RoundManager.Instance.round.ToString();
        Stat2.text = RoundManager.Instance.enemiesDefeated.ToString();
        Stat3.text = damageDealt.ToString();
        Stat4.text = towersPlaced.ToString();
        Stat5.text = MoneyManager.Instance.moneyGained.ToString();
        Stat6.text = MoneyManager.Instance.moneySpent.ToString();
        Stat7.text = CardManager.Instance.cardsPlayed.ToString();
        speedButtons.SetActive(false);
        StartCoroutine(WaitAndPause(1.0f));
    }

    public void ShowUpgrades()
    {
        upgradeUI.SetActive(true);
    }

    public void HideUpgrades()
    {
        upgradeUI.SetActive(false);
    }

    public void IncreaseCardUpgradeCost(string buttonText)
    {
        CardUpgradeCost.text = buttonText;
    }

    public IEnumerator WaitAndPause(float amount)
    {
        yield return new WaitForSeconds(amount);
        GameSpeedManager.Instance.SetGameSpeed(GameSpeed.PAUSE);
    }
}
