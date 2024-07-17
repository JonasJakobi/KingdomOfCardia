using UnityEngine;
public class Constants : Singleton<Constants>
{
    [SerializeField]
    private GameDifficulty gameDifficulty;
    public float towerCostGrowthRate = 1.7f;
    public float damageToEnemiesMultiplier = 1.0f;
    public float damageToTowersMultiplier = 1.0f;
    public float enemyMoveSpeedMultiplier = 1.0f;
    private void Start()
    {
        if (gameDifficulty != null)
        {
            LoadValues();
        }

    }

    private void LoadValues()
    {
        towerCostGrowthRate = gameDifficulty.towerCostGrowthRate;
        damageToEnemiesMultiplier = gameDifficulty.damageToEnemiesMultiplier;
        damageToTowersMultiplier = gameDifficulty.damageToTowersMultiplier;
        enemyMoveSpeedMultiplier = gameDifficulty.enemyMoveSpeedMultiplier;
    }
    public void SetGameDifficulty(GameDifficulty newGameDifficulty)
    {
        gameDifficulty = newGameDifficulty;
        LoadValues();
    }
}


