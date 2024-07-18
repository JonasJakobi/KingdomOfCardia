using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficulty", menuName = "GameDifficulty", order = 0)]
public class GameDifficulty : ScriptableObject
{
    public float towerCostGrowthRate = 1.7f;
    public float damageToEnemiesMultiplier = 1.0f;
    public float damageToTowersMultiplier = 1.0f;
    public float enemyMoveSpeedMultiplier = 1.0f;
    public int startMoney = 10;
}