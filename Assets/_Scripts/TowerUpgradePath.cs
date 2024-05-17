using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgradePath", menuName = "TowerUpgradePath", order = 2)]
public class TowerUpgradePath : ScriptableObject
{
    public TowerUpgrade[] upgrades;
}