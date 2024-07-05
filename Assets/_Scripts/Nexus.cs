using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : Singleton<Nexus>
{
    [SerializeField] private int health = 100;





    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Game over
            GameManager.Instance.ChangeGameState(GameState.GameOver);
            Debug.Log("Game Over Nexus!");
        }
    }
}
