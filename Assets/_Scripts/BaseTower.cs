using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all towers in the game. Registers on the grid and can take damage and be destroyed.
/// Specific implementations of towers should inherit from this class.
/// </summary>
public class BaseTower : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health = 100;

    [SerializeField] private bool isNexus = false;
    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;
        var tileHere = GridManager.Instance.GetTileAtPosition(transform.position);
        tileHere.SetHasBuilding(true, this.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        var tileHere = GridManager.Instance.GetTileAtPosition(transform.position);
        tileHere.SetHasBuilding(false, this.gameObject);

    }

    public void TakeDamage(int damage)
    {

        health -= damage;
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.Towers))
        {
            Debug.Log("Tower " + this.gameObject.name + ": " + health + "/" + maxHealth + " health left.");
        }
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }


    public bool GetIsNexus()
    {
        return isNexus;
    }
}



