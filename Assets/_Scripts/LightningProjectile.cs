using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A Projectile that instantly comes down upon an enemy and deals damage to it.
/// </summary>
public class LightningProjectile : MonoBehaviour, IProjectile
{
    public float delay = 0.04f;
    public float verticalDisplacement = 0.3f;
    Enemy enemy;
    public float movementSpeed = 5f;
    public int damage = 10;

    private void Update()
    {
        if (enemy != null)
        {
            transform.position = enemy.transform.position + verticalDisplacement * Vector3.up;
        }

    }

    public void SetValues(Enemy e, float speed, int damage, float lifetime, Quaternion rot)
    {
        enemy = e;
        movementSpeed = speed;
        this.damage = damage;
        transform.position = e.transform.position + verticalDisplacement * Vector3.up;
        StartCoroutine(DealDamageAndDestroy());
    }

    private IEnumerator DealDamageAndDestroy()
    {
        yield return new WaitForSeconds(delay);
        if (enemy != null)
        {
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Towers))
            {
                Debug.Log("Lightning projectile dealt damage to enemy");
            }
            enemy.TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }


}
