using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A tower that shoots homing projectiles at enemies. Projectiles will follow the enemy until it hits it and despawn on death.
/// </summary>
public class ProjectileTower : BaseTower
{
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    GameObject projectileSpawnPoint;
    Enemy currentTarget;
    [Tooltip("The range in which the tower can detect enemies in grid squares")]
    public int range = 5;
    [Tooltip("The delay between attacks in seconds")]
    [SerializeField]
    private float attackSpeed = 0.2f;
    [SerializeField]
    private int damage = 15;
    [SerializeField]
    private int projectileSpeed = 15;

    [SerializeField]
    private float projectileLifetime = 2f;

    [SerializeField] private float attackDelayTimer = 0;

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
        GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(WaitAndSpawnProjectile());
        StartCoroutine(AttackCooldown());
    }

    /// <summary>
    /// Waits for small delay to match animation, then spawns a projectile at the current target.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitAndSpawnProjectile()
    {
        var pos = projectileSpawnPoint.transform.position;
        //Wait first, then spawn
        yield return new WaitForSeconds(attackDelayTimer);
        //If the target is null, find a new one, if that fails, return
        if (currentTarget == null)
        {
            FindNewTarget();
            if (currentTarget == null)
            {
                yield break;
            }
        }
        var m = Instantiate(projectilePrefab, pos, Quaternion.identity);
        var rot = Quaternion.LookRotation(Vector3.forward, currentTarget.transform.position - projectileSpawnPoint.transform.position);
        m.GetComponent<IProjectile>().SetValues(currentTarget, projectileSpeed, damage, projectileLifetime, rot);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(range, range, 0));
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }


}
