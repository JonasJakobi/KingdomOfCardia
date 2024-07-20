using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : Singleton<Nexus>
{
    [SerializeField] private int health = 100;
    int damagingIndex = 0;
    public GameObject nexus1, nexus2, nexus3;

    private void FixedUpdate()
    {
        damagingIndex++;
        if (damagingIndex >= 10)
        {
            damagingIndex = 0;
            DealDamage();
        }
    }

    private void DealDamage()
    {
        //check adjacent fields for enemies, deal some damage to them
        BaseTower baseTower = GetComponent<BaseTower>();
        var adjacentTiles = GridManager.Instance.GetNeighbours(GridManager.Instance.GetTileAtPosition(transform.position));
        foreach (var tile in adjacentTiles)
        {
            if (tile.enemies.Count > 0)
            {
                tile.enemies.ForEach(e => e.TakeDamage(1));
            }
        }
        GridManager.Instance.GetTileAtPosition(transform.position).enemies.ForEach(e => e.TakeDamage(1));
    }

    //Aufgerufen im Roundmanager nach runde 15 und 30
    public void ChangeNexus(bool second)
    {
        if(second)
        {
            nexus1.SetActive(false);
            nexus2.SetActive(true);
            transform.GetComponent<BaseTower>().ChangeHealth(50000);
        }
        else
        {
            nexus2.SetActive(false);
            nexus3.SetActive(true);
            transform.GetComponent<BaseTower>().ChangeHealth(100000);
        }
    }
}
