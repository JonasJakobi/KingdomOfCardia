
using UnityEngine;

public interface IProjectile
{
    void SetValues(Enemy e, float speed, int damage, float lifetime, Quaternion rot);
}