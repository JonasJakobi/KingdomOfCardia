using UnityEngine;
public class Constants : Singleton<Constants>
{
    public static GameDifficulty selectedDifficulty;
    [SerializeField]
    private GameDifficulty gameDifficulty;
    public float towerCostGrowthRate = 1.7f;
    public float damageToEnemiesMultiplier = 1.0f;
    public float damageToTowersMultiplier = 1.0f;
    public float enemyMoveSpeedMultiplier = 1.0f;
    public int startMoney = 10;


    [Header("Global Constants untied to GameDifficulty")]
    public float lowestDamageToEnemiesMultiplier = 0.1f;
    private void Start()
    {
        if (selectedDifficulty != null)
        {
            gameDifficulty = selectedDifficulty;
        }
        if (gameDifficulty != null)
        {
            LoadValues();
        }

    }
    public void DecreaseDamageToEnemiesMultiplier()
    {
        damageToEnemiesMultiplier -= gameDifficulty.damageToEnemiesMultiplierDecreasePerRound;
        if (damageToEnemiesMultiplier < lowestDamageToEnemiesMultiplier)
        {
            damageToEnemiesMultiplier = lowestDamageToEnemiesMultiplier;
        }
    }

    private void LoadValues()
    {
        towerCostGrowthRate = gameDifficulty.towerCostGrowthRate;
        damageToEnemiesMultiplier = gameDifficulty.damageToEnemiesMultiplier;
        damageToTowersMultiplier = gameDifficulty.damageToTowersMultiplier;
        enemyMoveSpeedMultiplier = gameDifficulty.enemyMoveSpeedMultiplier;
        startMoney = gameDifficulty.startMoney;
    }
    public void SetGameDifficulty(GameDifficulty newGameDifficulty)
    {
        gameDifficulty = newGameDifficulty;
        LoadValues();
    }
}


