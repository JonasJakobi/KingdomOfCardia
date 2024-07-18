using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : Singleton<Nexus>
{
    [SerializeField] private int health = 100;
    int damagingIndex = 0;
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
}
