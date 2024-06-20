using UnityEditor.EditorTools;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgrade", menuName = "TowerUpgrade", order = 1)]
public class TowerUpgrade : ScriptableObject
{

    [Header("Main Stats")]
    public int health;
    public int cost;
    public int damage;
    [Tooltip("How many tiles away the tower can attack (square radius)")]
    public int range;
    [Tooltip("How many seconds between each attack")]
    public float attackSpeed;

    [Header("Stats for some towers")]
    public float projectileSpeed;
    public float projectileLifetime;
    public int maxEnemiesHittable;
    [Header("Visual stuff")]
    public string upgradeName;
    public Sprite upgradeSprite;
    public int starLevel;
    public StarColor starColor;

    protected int upgradeNr;
}

public enum StarColor
{
    Bronze,
    Silver,
    Gold,
    Black
}