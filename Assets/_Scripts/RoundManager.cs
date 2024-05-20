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
    [SerializeField] private int enemiesSpawned;
    [SerializeField] private int enemiesDefeated;

    [SerializeField] private GameObject EnemyType1;
    [SerializeField] private GameObject EnemyType2;
    [SerializeField] private GameObject EnemyType3;

    [Header("Spawn behaviour")]
    public float minSpawnTime = 1.0f;
    public float maxSpawnTime = 5.0f;

    [Header("Waves")]
    public int waveStart = 5;
    public int WaveDelay = 3;
    public float waveChance = 25.0f;
    [SerializeField] private bool waveQueued = false;
    [SerializeField] bool activeWave = false;
    [SerializeField] private int waveValue = 0;

    private void Start()
    {

    }

    private void Update()
    {
        if (roundValueLeft >= 3)
        {
            int randVal = Random.Range(1, 4);
            StartCoroutine(SpawnDelayCoroutine(randVal));
            roundValueLeft = roundValueLeft - randVal;
        }

        else if (roundValueLeft >= 2)
        {
            int randVal = Random.Range(1, 3);
            StartCoroutine(SpawnDelayCoroutine(randVal));
            roundValueLeft = roundValueLeft - randVal;
        }

        else if (roundValueLeft >= 1)
        {
            int randVal = 1;
            StartCoroutine(SpawnDelayCoroutine(randVal));
            roundValueLeft = roundValueLeft - randVal;
        }

        if (enemyCount <= 0)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length != 0)
            {
                enemyCount = enemies.Length;
            }

            else if ((enemies.Length == 0) && (roundValueLeft <= 0) && (activeWave == false) && (waveQueued == true))
            {
                Debug.Log("Welle geht gleich los!");
                StartCoroutine(WaveDelayCoroutine());
            }

            else if ((enemies.Length == 0) && (roundValueLeft <= 0) && (waveQueued == false))
            {
                NextRound();
            }
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
                enemyPrefab = EnemyType1;
                break;
            case 2:
                enemyPrefab = EnemyType2;
                break;
            case 3:
                enemyPrefab = EnemyType3;
                break;
            default:
                Debug.LogError("False enemyValue!");
                return;
        }

        Vector3 randomPos = new Vector3(randomWidth, Random.Range(0, height), 0);
        GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
    }

    private void QueueWave()
    {
        waveQueued = true;
        waveValue = Random.Range((roundValue / 3), (roundValue / 2));
        roundValueLeft -= waveValue;
    }

    //Start next round and decide if a wave will be present.
    private void NextRound()
    {
        activeWave = false;
        round++;
        enemyCount = 0;
        roundValue = roundValue * 2;
        roundValueLeft = roundValue;
        if (waveChance >= Random.Range(1.0f, 100.0f) && (round >= waveStart))
        {
            QueueWave();
        }
    }

    public void DefeatEnemy()
    {
        enemyCount--;
        enemiesDefeated++;
    }

    private IEnumerator SpawnDelayCoroutine(int randVal)
    {
        float maxDelay = maxSpawnTime;
        enemyCount++;

        if (round <= 2 || activeWave)
        {
            maxDelay = 2.0f;
        }

        float delay = Random.Range(minSpawnTime, maxDelay);

        yield return new WaitForSeconds(delay);
        SpawnEnemy(randVal);
        enemiesSpawned++;
    }

    private IEnumerator WaveDelayCoroutine()
    {
        activeWave = true;
        yield return new WaitForSeconds(WaveDelay);
        Debug.Log("Welle geht los!");
        enemyCount = 0;
        roundValueLeft = waveValue;
        waveQueued = false;
    }

}
