
using UnityEngine;

public interface IProjectile
{
    void SetValues(Enemy e, Quaternion rot, TowerUpgrade currentTowerUpgrade, BaseTower originTower);
}