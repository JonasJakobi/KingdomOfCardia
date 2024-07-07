using System.Collections;
using UnityEngine;
/// <summary>
/// A Projectile that instantly comes down upon an enemy and deals damage to it.
/// </summary>
public class LightningProjectile : MonoBehaviour, IProjectile
{
    [SerializeField]

    private RuntimeAnimatorController redLightningAnimatorController;

    public float delay = 0.04f;
    public float verticalDisplacement = 0.3f;
    Enemy enemy;
    public float movementSpeed = 5f;
    public int damage = 10;
    [SerializeField] private AudioClip projectileHitSound;

    private void Update()
    {
        if (enemy != null)
        {
            transform.position = enemy.transform.position + verticalDisplacement * Vector3.up;
        }

    }

    public void SetValues(Enemy e, Quaternion rot, TowerUpgrade towerStats)
    {
        enemy = e;
        movementSpeed = towerStats.projectileSpeed;
        this.damage = towerStats.damage;
        transform.position = e.transform.position + verticalDisplacement * Vector3.up;
        StartCoroutine(DealDamageAndDestroy());
        if (towerStats.upgradeName.Equals("Red Wizard"))
        {
            Debug.Log("Lightning will be red");
            Animator animator = GetComponent<Animator>();
            if (animator != null && redLightningAnimatorController != null)
            {
                animator.runtimeAnimatorController = redLightningAnimatorController;
            }
        }

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
            if (projectileHitSound != null)
            {
                AudioSystem.Instance.PlayProjectileSound(projectileHitSound);
            }
        }

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }


}
