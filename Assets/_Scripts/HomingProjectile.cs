using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Homing missile that follows its enemy target and deals damage to it once it reaches it.
/// </summary>
public class HomingProjectile : MonoBehaviour, IProjectile
{
    Enemy target;

    public float movementSpeed = 5f;
    public int damage = 10;
    [SerializeField] private AudioClip projectileHitSound;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;
            direction.Normalize();
            transform.position += direction * movementSpeed * Time.deltaTime;

            if (distance < 0.04f)
            {
                target.TakeDamage(damage);
                if (projectileHitSound != null)
                {
                    AudioSystem.Instance.PlayProjectileSound(projectileHitSound);
                }
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetValues(Enemy e, Quaternion rot, TowerUpgrade towerStats)
    {
        target = e;
        movementSpeed = towerStats.projectileSpeed;
        this.damage = towerStats.damage;
        transform.rotation = rot;
        if (towerStats.upgradeName == "Paradox")
        {
            Color color;
            if (ColorUtility.TryParseHtmlString("#380044", out color))
            {
                GetComponent<SpriteRenderer>().color = color;
            }
        }
        else if (towerStats.upgradeName == "Long Range")
        {
            Color color;
            if (ColorUtility.TryParseHtmlString("#E5CC12", out color))
            {
                GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

}
