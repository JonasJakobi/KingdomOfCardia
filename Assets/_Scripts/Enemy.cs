using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public int attackSpeed = 1;
    public int attackDamage = 10;
    [SerializeField] private float scale;

    private void Start()
    {
        this.transform.localScale = new Vector3(scale, scale, scale);
        GridManager.Instance.RegisterEnemyAtTile(this, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

    }
    public void MoveTowards(Vector3 target, float speed)
    {
        Vector3 pos = transform.position;
        Vector3 direction = target - pos;
        direction.Normalize();
        pos += direction * speed * Time.deltaTime;

        // Check if we entered a new tile, if so, register the enemy at the new tile and unregister at the old tile
        if (Mathf.FloorToInt(pos.x) != Mathf.FloorToInt(transform.position.x) || Mathf.FloorToInt(pos.y) != Mathf.FloorToInt(transform.position.y))
        {
            GridManager.Instance.UnregisterEnemyAtTile(this, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            GridManager.Instance.RegisterEnemyAtTile(this, Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        }
        transform.position = pos;
    }
}
