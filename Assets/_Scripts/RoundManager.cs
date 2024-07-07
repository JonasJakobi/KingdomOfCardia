using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine.UI;
using System.Linq;
//RoundManager to determine how many and which enemies are present in a round.

public class SpawnPoint
{
    public int widthPosition;
    public int heightPosition;
    public bool isFullWidth;

    public SpawnPoint(int widthPos, int heightPos, bool onFullWidth)
    {
        widthPosition = widthPos;
        heightPosition = heightPos;
        isFullWidth = onFullWidth;
    }
}
public class RoundManager : Singleton<RoundManager>
{
    [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    public Button startRoundButton;

    [SerializeField] private Animator panalAnimator;
    [SerializeField] private DifficultyDisplay difDis;

    [Header("Round Statistics")]
    public int round = 1;
    [SerializeField] private int roundValue = 1;
    [SerializeField] private int roundValueLeft = 0;
    [SerializeField] private int enemyCount = 0;
    [SerializeField] private int enemiesSpawned;
    public int enemiesDefeated;

    [SerializeField] private GameObject[] EnemyPrefabs;
    [SerializeField] private GameObject EnemyType1;
    [SerializeField] private GameObject EnemyType2;
    [SerializeField] private GameObject EnemyType3;



    [Header("Spawn behaviour")]
    public int maxSpawnPoints = 3;
    public float minSpawnTime = 1.0f;
    public float maxSpawnTime = 5.0f;

    [Header("Waves")]
    public int waveStart = 5;
    public int WaveDelay = 3;
    public float waveChance = 25.0f;
    [SerializeField] private bool waveQueued = false;
    [SerializeField] bool activeWave = false;
    [SerializeField] private int waveValue = 0;

    public GameObject gridManager;

    private void Start()
    {
        startRoundButton.onClick.AddListener(changeToPlayMode);
        roundValueLeft = 0;
        CreateSpawnPoints(1);
        var enemies = EnemyPrefabs.OrderBy(x => Mathf.Abs(x.GetComponentInChildren<Enemy>().GetValue()));
        foreach (var enemy in enemies)
        {
            Debug.Log(enemy.name + " ---  Value: " + enemy.GetComponentInChildren<Enemy>().GetValue());
        }

    }

    private void Update()
    {
        if (roundValueLeft > 0)
        {
            int maxValue = (roundValueLeft <= 4) ? 1 : roundValueLeft / 3; //biggest enemy is quarter of the roundValue, but restrict bottom value to 1
            int randVal = Random.Range(1, maxValue);
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

            else if ((enemies.Length == 0) && (roundValueLeft <= 0) && (waveQueued == false) && (GameManager.Instance.State == GameState.PlayMode))
            {
                GameManager.Instance.ChangeGameState(GameState.BuildMode);
            }
        }


    }

    public bool AttackableEnemiesAvailable()
    {
        return enemyCount > 0;
    }

    //Spawn an enemy
    private void SpawnEnemy(int enemyValue)
    {
        GameObject enemyPrefab = null;
        int randomIndex = Random.Range(0, spawnPoints.Count);
        float randomDistance = Random.Range(-1.0f, 1.0f);
        SpawnPoint spawnPoint = spawnPoints[randomIndex];
        int width = GridManager.WIDTH - 1;
        int height = GridManager.HEIGHT - 1;

        enemyPrefab = ChooseEnemeyPrefab(enemyValue);

        if (spawnPoint.isFullWidth)
        {
            if ((spawnPoint.widthPosition + randomDistance) >= width || (spawnPoint.widthPosition + randomDistance) <= 0)
            {
                randomDistance = -randomDistance;
            }
            Vector3 randomPos = new Vector3(spawnPoint.widthPosition + randomDistance, spawnPoint.heightPosition, 0);
            GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        }
        else
        {
            if ((spawnPoint.heightPosition + randomDistance) >= height || (spawnPoint.heightPosition + randomDistance) <= 0)
            {
                randomDistance = -randomDistance;
            }
            Vector3 randomPos = new Vector3(spawnPoint.widthPosition, spawnPoint.heightPosition + randomDistance, 0);
            GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        }
    }

    private GameObject ChooseEnemeyPrefab(int enemyValue)
    {
        return EnemyPrefabs.OrderBy(x => Mathf.Abs(x.GetComponentInChildren<Enemy>().GetValue() - enemyValue)).First(); //closest value to our wanted value, shuffled randomly
    }

    private void CreateSpawnPoints(int amount)
    {
        int width = GridManager.WIDTH - 1;
        int height = GridManager.HEIGHT - 1;
        int[] removeMiddleWidth = { 0, width };
        int[] removeMiddleHeight = { 0, height };

        spawnPoints.Clear();


        for (int i = 0; i < amount; i++)
        {
            bool onFullWidth = (Random.Range(0, 2) == 0);
            int randomIndex = Random.Range(0, 2);
            int randomHeight;
            int randomWidth;

            if (onFullWidth)
            {
                randomHeight = removeMiddleHeight[randomIndex];
                randomWidth = Random.Range(0, width);
            }
            else
            {
                randomWidth = removeMiddleWidth[randomIndex];
                randomHeight = Random.Range(0, height);
            }

            if (CheckForTreesAndMountains(randomWidth, randomHeight))
            {
                spawnPoints.Add(new SpawnPoint(randomWidth, randomHeight, onFullWidth));
                UIChangeManager.Instance.CreateWaveAlert(spawnPoints[i]);
                Debug.Log("New SpawnPoint: " + randomWidth + ", " + randomHeight + ", " + onFullWidth);
            }
            else
            {
                i -= 1;
            }


        }
    }

    private bool CheckForTreesAndMountains(int x, int y)
    {
        foreach (Transform go in gridManager.transform)
        {
            if (go.transform.position.x == x && go.transform.position.y == y)
            {
                if (go.GetComponent<FlowFieldTile>().GetAssignedCost() == 10000)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void QueueWave()
    {
        waveQueued = true;
        waveValue = Random.Range((roundValue / 3), (roundValue / 2));
        roundValueLeft -= waveValue;
    }

    //Prepare the next wave
    public void NextRound()
    {

        activeWave = false;
        round++;
        UIChangeManager.Instance.UpdateRound();
        enemyCount = 0;
        roundValue = roundValue * 2;
        if (round <= 3)
        {
            CreateSpawnPoints(1);
        }
        else
        {
            CreateSpawnPoints(Random.Range(1, maxSpawnPoints));
        }
    }

    //Start next round and decide if a wave will be present
    public void BeginNextRound()
    {
        panalAnimator.SetBool("gone", false);
        difDis.Changes();
        panalAnimator.SetBool("gone", true);
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
        Debug.Log("Wave!");
        enemyCount = 0;
        roundValueLeft = waveValue;
        AudioSystem.Instance.PlayDramaticBoom();
        waveQueued = false;
    }

    [ProButton]
    private void changeToPlayMode()
    {
        roundValueLeft = roundValue;
        GameManager.Instance.ChangeGameState(GameState.PlayMode);
    }

}

