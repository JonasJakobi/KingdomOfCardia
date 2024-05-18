using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgrade", menuName = "TowerUpgrade", order = 1)]
public class TowerUpgrade : ScriptableObject
{

    [Header("Main Stats")]
    public int health;
    public int cost;
    public int damage;
    public int range;
    public float attackSpeed; // Attack speed in attacks per second

    [Header("Stats for some towers")]
    public float projectileSpeed;
    public float projectileLifetime;
    public int maxEnemiesHittable;
    [Header("Visual stuff")]
    public string upgradeName;
    public Sprite upgradeSprite;
    public int starLevel;
    public StarColor starColor;
}

public enum StarColor
{
    Bronze,
    Silver,
    Gold,
    Black
}