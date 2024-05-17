using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgrade", menuName = "TowerUpgrade", order = 1)]
public class TowerUpgrade : ScriptableObject
{
    public string upgradeName;
    public int health;
    public int cost;
    public float damage;
    public int range;
    public float attackSpeed; // Attack speed in attacks per second
}