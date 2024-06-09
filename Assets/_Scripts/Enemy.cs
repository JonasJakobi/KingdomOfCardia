using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
/// <summary>
/// Base class for all enemies in the game.
/// Can move on the grid and attack buildings.
/// </summary>
public class Enemy : MonoBehaviour
{


    [SerializeField] private float scale;





    [Header("Stats")]
    public int maxHealth = 100;


    public float movementSpeed = 1;
    [Tooltip("How many seconds between each attack")]
    public int attackCooldown = 1;
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

    private Animator animator;

    private void Start()
    {
        health = maxHealth;
        this.transform.localScale = new Vector3(scale, scale, scale);
        GridManager.Instance.RegisterEnemyAtTile(this, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        currentTile = GridManager.Instance.GetTileAtPosition(transform.position);
        animator = GetComponent<Animator>();

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
        }
        else if (buildingVector != Vector3.zero)//move towards building if possible
        {
            animator.SetBool("Attack", false);
            MoveInDirection((buildingVector - transform.position).normalized, movementSpeed);
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
    /// <summary>
    /// Moves the enemy in the given direction (direction vector) with the given speed.
    /// Will only move for the current frame.
    /// </summary>
    /// <param name="direction">Direction Vector (normalized)</param>
    /// <param name="speed">how far to move during this frame</param>
    public void MoveInDirection(Vector3 direction, float speed)
    {
        Vector3 pos = transform.position;
        pos += direction * speed * Time.deltaTime;

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
        GameObject roundManagerObject = GameObject.Find("RoundManager");
        RoundManager damageEnemy = roundManagerObject.GetComponent<RoundManager>();
        health -= damage;
        if (health <= 0)
        {
            damageEnemy.DefeatEnemy();
            currentTile.UnregisterEnemy(this);
            MoneyManager.Instance.AddMoney(GetValue());
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Tries to attack the building on the current tile.
    /// </summary>
    protected virtual void Attack()
    {


        var building = currentTile.GetBuilding();
        if (building != null)
        {
            building.GetComponent<BaseTower>().TakeDamage(attackDamage);
            canAttack = false;
            StartCoroutine(AttackCooldown());
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Enemies))
            {
                Debug.Log("Enemy attacked building" + building + " for " + attackDamage + " damage.");
            }
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

    public void SlowEnemy(int slow, float duration)
    {
        float slowMultiplicator = 1f / (float)slow;
        StartCoroutine(SlowForSeconds(slowMultiplicator, duration));
    }

    public IEnumerator SlowForSeconds(float slowMultiplicator, float duration)
    {
        float originalMovementSpeed = movementSpeed;
        movementSpeed = movementSpeed * slowMultiplicator;
        yield return new WaitForSeconds(duration);
        movementSpeed = originalMovementSpeed;
    }

}
