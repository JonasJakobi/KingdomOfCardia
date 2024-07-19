using System.Collections;
using System.Collections.Generic;

using UnityEngine;
/// <summary>
/// Base class for all enemies in the game.
/// Can move on the grid and attack buildings.
/// </summary>
public class Enemy : MonoBehaviour
{
    public Renderer objectRenderer;


    public Color originalColor = Color.white;

    public Color freezeColor = Color.cyan;

    public Color hitColor = Color.red;

    public Color reduceColor = Color.magenta;

    public Color overallSlow = Color.grey;

    [SerializeField] private AudioClip enemySound;

    public float enemySoundChance = 0.2f;


    [Header("Stats")]
    public int maxHealth = 100;
    private float originalMovementSpeed = 1;


    public float movementSpeed = 1;
    [Tooltip("How many seconds between each attack")]
    public float attackCooldown = 1;
    [Tooltip("How much damage the enemy does per attack")]
    public int attackDamage = 10;
    [Tooltip("How far the enemy can attack from (1 = 1 tile)")]
    public float attackRange = 0.1f;
    [Header("Debugging Info")]
    [SerializeField] private int health = 100;
    [SerializeField] Tile currentTile;

    [SerializeField] private bool attacking = false;

    [SerializeField] private bool canAttack = true;
    [SerializeField] private BaseTower currentlyTargetedBuilding;

    [SerializeField] private float damageValueRatio = 1.0f;

    [SerializeField] private GameObject firePrefab, necroticPrefab, electricPrefab, splatterPrefab;

    private Animator animator;

    private int originalAttackDamage;


    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        health = maxHealth;
        originalMovementSpeed = movementSpeed;
        originalAttackDamage = attackDamage;
        GridManager.Instance.RegisterEnemyAtTile(this, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        currentTile = GridManager.Instance.GetTileAtPosition(transform.position);
        animator = GetComponent<Animator>();
        RandomSoundChance();

    }
    private void Update()
    {
        //If we are not on a tile, register at the tile we are on
        if (currentTile == null)
        {
            currentTile = GridManager.Instance.GetTileAtPosition(transform.position);
        }
        //Check if we Attack or Move.
        var nexusVector = currentTile.flowFieldTile.GetNexusMovementVector();
        var buildingVector = currentTile.flowFieldTile.GetTowerMovementVector();

        if (nexusVector != Vector3.zero)//move towards nexus if possible
        {
            animator.SetBool("Attack", false);
            MoveInDirection((nexusVector - transform.position).normalized, movementSpeed);
            if (nexusVector.x > transform.position.x)
            {
                transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            }
            else
            {
                transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            }
        }
        else if (buildingVector != Vector3.zero)//move towards building if possible
        {
            animator.SetBool("Attack", false);
            MoveInDirection((buildingVector - transform.position).normalized, movementSpeed);
            UIChangeManager.Instance.QueueWarning();
        }
        else
        {
            MoveToOrAttackBuilding();
        }


    }
    /// <summary>
    /// Called when there is no movement vector on the current tile. 
    /// Will try to attack the building on the tile or move towards it to get in range to attack.
    /// </summary>
    private void MoveToOrAttackBuilding()
    {

        if (currentlyTargetedBuilding == null)
        {
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Enemies))
            {
                Debug.Log("Enemy has no building to target, finding new target...");
            }
            currentlyTargetedBuilding = GridManager.Instance.FindNearestBuilding(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }
        else
        {
            if (Vector3.Distance(transform.position, currentlyTargetedBuilding.transform.position) > attackRange)
            {
                animator.SetBool("Attack", false);
                MoveInDirection((currentlyTargetedBuilding.transform.position - transform.position).normalized, movementSpeed);
            }
            else
            {
                animator.SetBool("Attack", true);

                if (canAttack)
                {
                    Attack();
                }

            }
        }
    }
    private void SpawnEffekt(GameObject animationPrefab, float duration)
    {
        if (animationPrefab != null)
        {
            var prefab = Instantiate(animationPrefab, transform);
            Destroy(prefab, duration);
        }
    }
    /// <summary>
    /// Methods for starting fire damage and the corresponding particle system  
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="damage"></param>

    public void StartFireDamage(float duration, int damage)
    {
        SpawnEffekt(firePrefab, duration);
        StartCoroutine(TakeOverTimeDamage(duration, damage));

    }

    private IEnumerator TakeOverTimeDamage(float duration, int damage)
    {
        for (int i = 0; i < duration; i++)
        {
            TakeDamage(damage);
            yield return new WaitForSeconds(1);
        }
    }


    /// <summary>
    /// Methods for starting necrotic damage and the corresponding particle system  
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="damage"></param>

    public void StartNecroticDamage(float duration, int damage)
    {
        SpawnEffekt(necroticPrefab, duration);
        StartCoroutine(TakeNecroticDamageOverTime(duration, damage));
    }

    private IEnumerator TakeNecroticDamageOverTime(float duration, int damage)
    {
        for (int i = 0; i < duration; i++)
        {
            int exponentialDamage = damage * (int)Mathf.Pow(2, i);
            TakeDamage(exponentialDamage);
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// Methods for starting electric damage and the corresponding particle system  
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="damage"></param>

    public void StartElectricDamage(float duration, int damage)
    {
        SpawnEffekt(electricPrefab, duration);
        StartCoroutine(TakeElectricDamageOverTime(duration, damage));
    }

    public IEnumerator TakeElectricDamageOverTime(float duration, int damage)
    {
        TakeDamage(damage * 5);
        for (int i = 0; i < duration; i++)
        {
            TakeDamage(damage / 2);
            yield return new WaitForSeconds(1);
        }
    }
    public void ReduceDamage(float reduce, float duration)
    {
        StartCoroutine(ReduceDamageCoroutine(reduce, duration));
    }

    private IEnumerator ReduceDamageCoroutine(float reduce, float duration)
    {
        attackDamage = Mathf.RoundToInt(originalAttackDamage / reduce);
        //Change Color to purple
        StartCoroutine(ChangeColor(reduceColor, false, 0f));

        yield return new WaitForSeconds(duration);

        //restore original damage
        attackDamage = originalAttackDamage;

        //revert Color to normal
        StartCoroutine(ChangeColor(originalColor, false, 0f));


    }


    /// <summary>
    /// Moves the enemy in the given direction (direction vector) with the given speed.
    /// Will only move for the current frame.
    /// </summary>
    /// <param name="direction">Direction Vector (normalized)</param>
    /// <param name="speed">how far to move during this frame</param>
    public void MoveInDirection(Vector3 direction, float speed)
    {
        Vector3 pos = transform.position;
        pos += direction * speed * Time.deltaTime * Constants.Instance.enemyMoveSpeedMultiplier;

        // Check if we entered a new tile, if so, register the enemy at the new tile and unregister at the old tile
        if (Mathf.RoundToInt(pos.x) != Mathf.RoundToInt(transform.position.x) || Mathf.RoundToInt(pos.y) != Mathf.RoundToInt(transform.position.y))
        {
            GridManager.Instance.UnregisterEnemyAtTile(this, Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            GridManager.Instance.RegisterEnemyAtTile(this, Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));


        }
        transform.position = pos;
        currentTile = GridManager.Instance.GetTileAtPosition(transform.position);
    }

    public void TakeDamage(int damage)
    {
        int trueDamage = Mathf.RoundToInt(damage * Constants.Instance.damageToEnemiesMultiplier);
        RandomSoundChance();
        UIChangeManager.Instance.damageDealt += trueDamage;
        StartCoroutine(ChangeColor(hitColor, true, 0.2f));
        health -= trueDamage;
        if (health <= 0)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        var splatter = Instantiate(splatterPrefab, transform);
        splatter.transform.SetParent(null);
        Destroy(splatter, 1);
        AudioSystem.Instance.PlaySplash();
        RoundManager.Instance.DefeatEnemy();
        Debug.Log("Gegner besiegt!");
        currentTile.UnregisterEnemy(this);
        MoneyManager.Instance.AddMoney(GetValue());
    }
    /// <summary>
    /// Tries to attack the building on the current tile.
    /// </summary>
    protected virtual void Attack()
    {

        RandomSoundChance();
        currentlyTargetedBuilding.TakeDamage(attackDamage);
        canAttack = false;
        StartCoroutine(AttackCooldown());
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Enemies))
        {
            Debug.Log("Enemy attacked building" + currentlyTargetedBuilding + " for " + attackDamage + " damage.");
        }

    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    /// <summary>
    /// Generate and return the value of the enemy.
    /// </summary>
    /// <returns>The value / amount of money we get from this enemy.</returns>
    public int GetValue()
    {
        float value = maxHealth * (damageValueRatio * attackDamage / attackCooldown);
        value = value / 1000;
        return Mathf.RoundToInt(value);

    }

    public void SlowEnemy(float slow, float duration, bool musicEffect)
    {
        float slowMultiplicator = 1f / slow;
        StartCoroutine(SlowForSeconds(slowMultiplicator, duration, musicEffect));
    }

    public IEnumerator SlowForSeconds(float slowMultiplicator, float duration, bool musicEffect)
    {

        StartCoroutine(ChangeColor(freezeColor, false, 0.0f));

        if (musicEffect)
        {
            AudioSystem.Instance.ChangePitch(AudioSystem.Instance.originalPitch - (0.75f * slowMultiplicator));
        }

        movementSpeed = movementSpeed * slowMultiplicator;

        yield return new WaitForSeconds(duration);

        if (musicEffect)
        {
            AudioSystem.Instance.ChangePitch(AudioSystem.Instance.originalPitch);
        }

        movementSpeed = originalMovementSpeed;
        StartCoroutine(ChangeColor(originalColor, false, 0.0f));


    }

    public void SlowMovementAndAttack(float slow, float duration)
    {
        float slowMultiplicator = 1f / (float)slow;
        StartCoroutine(SlowForSeconds(slowMultiplicator, duration, true));
        StartCoroutine(SlowAttackCooldown(duration));
    }

    private IEnumerator SlowAttackCooldown(float duration)
    {
        float originalCooldown = attackCooldown;
        attackCooldown *= 2; //doubles AttackCooldown
        yield return new WaitForSeconds(duration);
        attackCooldown = originalCooldown;
    }

    public IEnumerator ChangeColor(Color color, bool revertColor, float duration)
    {


        Color savedColor = objectRenderer.material.color;
        objectRenderer.material.color = color;

        if (objectRenderer != null && revertColor)
        {
            yield return new WaitForSeconds(duration);
            if (savedColor != hitColor)
            {
                objectRenderer.material.color = savedColor;
            }
            else
            {
                objectRenderer.material.color = originalColor;
            }

        }

    }

    public void RandomSoundChance()
    {
        if (enemySound != null)
        {
            if (Random.Range(0.0f, 1.0f) >= (1.0f - enemySoundChance))
            {
                AudioSystem.Instance.PlayEnemySound(enemySound);
            }

        }

    }

}
