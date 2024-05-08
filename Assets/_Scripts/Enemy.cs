using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public int attackSpeed = 1;
    public float movementSpeed = 1;
    public int attackDamage = 10;
    [SerializeField] private float scale;

    [SerializeField]
    Tile currentTile;

    private void Start()
    {
        this.transform.localScale = new Vector3(scale, scale, scale);
        GridManager.Instance.RegisterEnemyAtTile(this, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        currentTile = GridManager.Instance.GetTileAtPosition(transform.position);

    }
    private void Update()
    {
        if (currentTile == null)
        {
            currentTile = GridManager.Instance.GetTileAtPosition(transform.position);
        }
        var vector = currentTile.GetEnemyMovementVector();
        if (vector == Vector3.zero)
        {
            //Attack
        }
        else
        {
            MoveInDirection(currentTile.GetEnemyMovementVector(), movementSpeed);
        }


    }
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
        health -= damage;
        if (health <= 0)
        {
            currentTile.UnregisterEnemy(this);
            Destroy(gameObject);
        }
    }



}
