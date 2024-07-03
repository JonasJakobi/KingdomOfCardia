using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// A projectile that deals damage to enemies it collides with.
/// </summary>
public class CollidingProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private bool destroysOnImpact = true;
    public float movementSpeed = 5f;
    public int damage = 10;
    /// <summary>
    /// After how many seconds the projectile will be destroyed.
    /// </summary>
    public float lifetime = 2f;
    /// <summary>
    /// The maximum number of enemies this projectile can hit before being destroyed.
    /// Gets set at start and then decremented each time an enemy is hit.
    /// </summary>
    public int maxEnemiesStillHitable = 1;

    private void Update()
    {
        transform.position += transform.up * movementSpeed * Time.deltaTime;
    }

    public void SetValues(Enemy e, Quaternion rot, TowerUpgrade towerStats)
    {
        movementSpeed = towerStats.projectileSpeed;
        damage = towerStats.damage;
        lifetime = towerStats.projectileLifetime;
        transform.rotation = rot;
        maxEnemiesStillHitable = towerStats.maxEnemiesHittable;
        StartCoroutine(DestroyAfterLifetime());
    }
    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponentInChildren<Enemy>() != null)
        {
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Towers))
            {
                Debug.Log("Colliding projectile collided with enemy");
            }

            //Only if all of these remain ice projectiles:
            other.gameObject.GetComponentInChildren<Enemy>().SlowEnemy(2, 1.5f, false);
            other.gameObject.GetComponentInChildren<Enemy>().TakeDamage(damage);

            if (destroysOnImpact)
            {
                Destroy(gameObject);
            }
            maxEnemiesStillHitable--;
            if (maxEnemiesStillHitable <= 0)
            {
                Destroy(gameObject);
            }

        }
    }

}
