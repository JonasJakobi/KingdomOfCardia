using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
//RoundManager to determine how many and which enemies are present in a round.

public class RoundManager : MonoBehaviour
{
    [Header("Round Statistics")]
    public int round = 1;
    [SerializeField] private int roundValue = 1;
    [SerializeField] private int roundValueLeft = 1;
    [SerializeField] private int enemyCount = 0;
    private int enemiesSpawned;
    private int enemiesDefeated;

    [SerializeField] private GameObject TestObject1;
    [SerializeField] private GameObject TestObject2;
    [SerializeField] private GameObject TestObject3;

    private void Start()
    {

    }

    private void Update()
    {

        if (roundValueLeft >= 3)
        {
            int randVal = Random.Range(1, 4);
            SpawnEnemy(randVal);
            roundValueLeft = roundValueLeft - randVal;
        }

        else if (roundValueLeft >= 2)
        {
            int randVal = Random.Range(1, 3);
            SpawnEnemy(randVal);
            roundValueLeft = roundValueLeft - randVal;
        }

        else if (roundValueLeft >= 1)
        {
            int randVal = 1;
            SpawnEnemy(randVal);
            roundValueLeft = roundValueLeft - randVal;
        }

        if ((enemyCount <= 0) && (roundValueLeft == 0))
        {
            NextRound();
        }
    }

    //Spawn an enemy
    private void SpawnEnemy(int enemyValue)
    {

        int width = GridManager.WIDTH - 1;
        int height = GridManager.HEIGHT;
        GameObject enemyPrefab = null;
        int[] spawnWidth = { 0, width };
        int randomIndex = Random.Range(0, 2);
        int randomWidth = spawnWidth[randomIndex];

        switch (enemyValue)
        {
            case 1:
                enemyPrefab = TestObject1;
                break;
            case 2:
                enemyPrefab = TestObject2;
                break;
            case 3:
                enemyPrefab = TestObject3;
                break;
            default:
                Debug.LogError("False enemyValue!");
                return;
        }

        Vector3 randomPos = new Vector3(randomWidth, Random.Range(0, height), 0);
        //Instantiate(testEnemy, randomPos, Quaternion.identity);

        //Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 2.0f;
        GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);

        enemyCount++;

        float spawnDelay = Random.Range(1.0f, 3.0f);

        StartCoroutine(SpawnDelayCoroutine(spawnDelay));
    }

    private IEnumerator SpawnDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private void NextRound()
    {
        round++;
        roundValue = roundValue * 2;
        roundValueLeft = roundValue;
    }

    public void DefeatEnemy()
    {
        enemyCount--;
    }
}
