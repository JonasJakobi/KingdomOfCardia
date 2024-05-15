using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A projectile that deals damage to enemies it collides with.
/// </summary>
public class CollidingProjectile : MonoBehaviour
{
    [SerializeField] private bool destroysOnImpact = true;
    public float movementSpeed = 5f;
    public int damage = 10;

    public float lifetime = 2f;

    private void Update()
    {
        transform.position += transform.up * movementSpeed * Time.deltaTime;
    }

    public void SetValues(float speed, int damage, float lifetime, Quaternion rotation)
    {

        movementSpeed = speed;
        this.damage = damage;
        this.lifetime = lifetime;
        transform.rotation = rotation;
        StartCoroutine(DestroyAfterLifetime());
    }
    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Towers))
            {
                Debug.Log("Colliding projectile collided with enemy");
            }

            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            if (destroysOnImpact)
            {
                Destroy(gameObject);
            }

        }
    }
}
