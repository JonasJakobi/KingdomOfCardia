using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Homing missile that follows its enemy target and deals damage to it once it reaches it.
/// </summary>
public class HomingProjectile : MonoBehaviour
{
    Enemy target;

    public float movementSpeed = 5f;
    public int damage = 10;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;
            direction.Normalize();
            transform.position += direction * movementSpeed * Time.deltaTime;

            if (distance < 0.02f)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetValues(Enemy e, float speed, int damage)
    {
        target = e;
        movementSpeed = speed;
        this.damage = damage;
    }

}
