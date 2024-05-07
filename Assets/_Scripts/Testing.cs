using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
public class Testing : MonoBehaviour
{
    public GameObject testEnemy;

    [ProButton]
    public void SpawnRandomTestEnemies(int amount)
    {
        int width = GridManager.WIDTH;
        int height = GridManager.HEIGHT;

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(0, width), Random.Range(0, height), 0);
            Instantiate(testEnemy, randomPos, Quaternion.identity);
        }
    }
}
