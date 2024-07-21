using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] Enemy currentTarget;

    [SerializeField] private float attackDelayTimer = 0;

    [SerializeField] private bool canAttack = true;
    [SerializeField]
    private TargetingType targetingType;
    [SerializeField] private AudioClip towerShootSound;
    [SerializeField] private AudioClip towerWindupSound;
    // Update is called once per frame
    private Animator anim;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        if (canAttack && RoundManager.Instance.AttackableEnemiesAvailable())
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
        currentTarget = GridManager.Instance.FindEnemy((transform.position.x), (transform.position.y), currentUpgrade.range, targetingType);

    }

    private void ShootAtCurrentTarget()
    {
        if (towerWindupSound != null)
        {
            AudioSystem.Instance.PlayTowerSound(towerWindupSound);
        }
        anim.SetTrigger("Attack");
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
        if (towerShootSound != null)
        {
            AudioSystem.Instance.PlayTowerSound(towerShootSound);
        }
        var m = Instantiate(projectilePrefab, pos, Quaternion.identity);
        var rot = Quaternion.LookRotation(Vector3.forward, currentTarget.transform.position - projectileSpawnPoint.transform.position);
        m.GetComponent<IProjectile>().SetValues(currentTarget, rot, currentUpgrade);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(currentUpgrade.range, currentUpgrade.range, 0));
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(currentUpgrade.attackSpeed);
        canAttack = true;
    }


}
