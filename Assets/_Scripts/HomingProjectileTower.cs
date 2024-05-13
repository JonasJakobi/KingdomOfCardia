using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A tower that shoots homing projectiles at enemies. Projectiles will follow the enemy until it hits it and despawn on death.
/// </summary>
public class HomingProjectileTower : BaseTower
{
    [SerializeField]
    GameObject projectilePrefab;

    Enemy currentTarget;
    [Tooltip("The range in which the tower can detect enemies in grid squares")]
    public int range = 5;
    [Tooltip("The delay between attacks in seconds")]
    [SerializeField]
    private float attackDelay = 0.2f;
    [SerializeField]
    private int damage = 15;
    [SerializeField]
    private int projectileSpeed = 15;

    private bool canAttack = true;
    [SerializeField]
    private TargetingType targetingType;

    // Update is called once per frame
    void Update()
    {
        if (canAttack)
        {
            FindNewTarget();
            if (currentTarget != null)
            {
                ShootAtCurrentTarget();
            }
        }
    }

    private void FindNewTarget()
    {
        Enemy e = GridManager.Instance.FindEnemy(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), range, targetingType);
        if (e != null)
        {
            currentTarget = e;
        }
    }

    private void ShootAtCurrentTarget()
    {
        var m = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        m.GetComponent<HomingProjectile>().SetValues(currentTarget, projectileSpeed, damage);

        StartCoroutine(AttackCooldown());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(range, range, 0));
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }


}
