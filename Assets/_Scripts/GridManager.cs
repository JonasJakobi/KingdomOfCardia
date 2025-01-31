using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
/// <summary>
/// Basic GridManager to handle the grid of the game. Can be used to get tiles at positions, register and unregister enemies and find enemies.
/// </summary>
public class GridManager : Singleton<GridManager>

{
    public float mountainChance = 0.05f;
    public float mountainChanceMultiplier = 170.0f;
    public float forestChance = 0.1f;
    public float forestChanceMultiplier = 90.0f;
    [SerializeField] private int startX, startY;
    public const int WIDTH = 32;
    public const int HEIGHT = 18;
    public GameObject mainTower;
    [SerializeField] private List<Tile> tilePrefab;
    [SerializeField] private Tile mountainTilePrefab, forestTilePrefab;

    [SerializeField] private Transform camera;


    public Tile[,] grid;
    private void Start()
    {
        GenerateGrid();
    }
    void GenerateGrid()
    {
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.General))
        {
            Debug.Log("Generating Grid...");
        }
        grid = new Tile[WIDTH, HEIGHT];
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Tile prefab = tilePrefab[3];
                if (y % 2 == 0 && x % 2 == 0)
                {
                    prefab = tilePrefab[3];
                }
                else if (y % 2 == 0 && x % 2 != 0)
                {
                    prefab = tilePrefab[2];
                }
                else if (y % 2 != 0 && x % 2 == 0)
                {
                    prefab = tilePrefab[2];
                }


                grid[x, y] = Instantiate(prefab, new Vector3(x + startX, y + startY, 0), Quaternion.identity).GetComponent<Tile>();
                grid[x, y].name = $"Tile {x} {y}";
                grid[x, y].transform.parent = transform;

                //Spownt aber spownt nichts auf dem Hauptturm
                if (mainTower.transform.position.x != x || mainTower.transform.position.y != y)
                {
                    bool hasNeighbouringMountain = false;
                    bool hasNeighbouringForest = false;

                    foreach (Tile tile in GetNeighbours(grid[x, y]))
                    {
                        if (tile != null && tile.Type == Tile.TileType.Mountain)
                        {
                            Debug.Log("tile: " + tile.Type);
                            hasNeighbouringMountain = true;
                            break;
                        }
                        else if (tile != null && tile.Type == Tile.TileType.Forest)
                        {
                            hasNeighbouringForest = true;
                            break;
                        }

                    }
                    if (x == 0 || y == 0 || x == WIDTH - 1 || y == HEIGHT - 1)
                    {
                        grid[x, y].SetIsBuildable(false);
                        continue;
                    }
                    //Small chance for mountain, otherwise normal tile
                    if ((!hasNeighbouringMountain && Random.Range(0, 100) < mountainChance * 10) || (hasNeighbouringMountain && Random.Range(0, 100) < mountainChance * mountainChanceMultiplier))
                    {

                        grid[x, y] = Instantiate(mountainTilePrefab, new Vector3(x + startX, y + startY, 0), Quaternion.identity).GetComponent<Tile>();
                        grid[x, y].name = $"Tile {x} {y}";
                        grid[x, y].transform.parent = transform;
                        grid[x, y].Type = Tile.TileType.Mountain;
                    }
                    else if ((!hasNeighbouringForest && Random.Range(0, 100) < forestChance * 10) || ((hasNeighbouringForest || hasNeighbouringMountain) && Random.Range(0, 100) < forestChance * forestChanceMultiplier))
                    {

                        grid[x, y] = Instantiate(forestTilePrefab, new Vector3(x + startX, y + startY, 0), Quaternion.identity).GetComponent<Tile>();
                        grid[x, y].name = $"Tile {x} {y}";
                        grid[x, y].transform.parent = transform;
                        grid[x, y].Type = Tile.TileType.Forest;
                    }

                }





            }
        }
        camera.position = new Vector3((WIDTH / 2) + 2, (HEIGHT / 2), -10);
        //Generate list of all FlowFieldTiles from grid[,,]
        List<FlowFieldTile> flowFieldTiles = new List<FlowFieldTile>();
        foreach (Tile tile in grid)
        {
            if (tile != null)
            {
                flowFieldTiles.Add(tile.GetComponent<FlowFieldTile>());
            }
        }
        FlowFieldGenerator.Instance.InitializeFlowField(flowFieldTiles);

    }



    public Tile GetTileAtPosition(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x - startX);
        int y = Mathf.RoundToInt(pos.y - startY);

        if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT)
        {
            //Debug.LogError("Tile out of bounds for pos: " + pos + " and x, y: " + x + "," + y);
            //Debug.LogError("Tile out of bounds");
            return null;
        }
        return grid[x, y];
    }

    public Tile GetTileAtPosition(int x, int y)
    {
        return grid[x, y];
    }




    public void UnregisterEnemyAtTile(Enemy enemy, int x, int y)
    {
        if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT)
        {
            Debug.LogError("Cant UnRegister Enemy at " + x + y + " , Tile out of bounds");
            return;
        }
        else
        {
            grid[x, y].UnregisterEnemy(enemy);
        }

    }

    public void RegisterEnemyAtTile(Enemy enemy, int x, int y)
    {
        if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT)
        {
            Debug.Log("Cant register enemy at " + x + y + " , Tile out of bounds, registerint at closest point instead");
            //put at closest tile?
            grid[Mathf.Clamp(x, 0, WIDTH - 1), Mathf.Clamp(y, 0, HEIGHT - 1)].RegisterEnemy(enemy);
            return;
        }
        else
        {
            grid[x, y].RegisterEnemy(enemy);
        }

    }

    public Enemy FindEnemy(float x, float y, int maxDistance, TargetingType targetingType = TargetingType.Nächsten)
    {
        Enemy bestEnemySoFar = null;
        double bestValueSoFar = float.MaxValue;
        Vector3 nexusPos = Nexus.Instance.transform.position;
        foreach (Enemy enemy in RoundManager.Instance.livingEnemies)
        {
            if (enemy == null)
            {
                continue;
            }
            float distance = Vector3.Distance(enemy.transform.position, new Vector3(x, y, 0));
            if (distance > maxDistance)
            {

                continue;
            }
            switch (targetingType)
            {
                case TargetingType.Ersten:
                    float nexusDistance = Vector3.Distance(enemy.transform.position, nexusPos);
                    if (nexusDistance < bestValueSoFar)
                    {
                        bestValueSoFar = nexusDistance;
                        bestEnemySoFar = enemy;
                    }
                    break;
                case TargetingType.Nächsten:
                    if (distance < bestValueSoFar)
                    {
                        bestValueSoFar = distance;
                        bestEnemySoFar = enemy;
                    }
                    break;
                case TargetingType.Stärksten:
                    if (1 / enemy.health < bestValueSoFar)
                    {
                        bestValueSoFar = 1 / enemy.health;
                        bestEnemySoFar = enemy;
                    }
                    break;
            }
        }
        return bestEnemySoFar;
    }
    public BaseTower FindNearestBuilding(int x, int y)
    {
        List<BaseTower> towers = new List<BaseTower>();
        int maxDistance = 12; // Should be enough to cover all towers as this shouldnt ever be >1, as enemies only search when they are on a tile next to a tower
        for (int d = 0; d < maxDistance; d++)
        {
            towers.Clear();
            // Loop through a square around the enemy (x,y)
            for (int i = -d; i <= d; i++)
            {
                for (int j = -d; j <= d; j++)
                {
                    int checkX = x + i;
                    int checkY = y + j;
                    if (checkX < 0 || checkX >= WIDTH || checkY < 0 || checkY >= HEIGHT) continue; // Skip out-of-bounds tiles   
                    else if (grid[checkX, checkY].HasBuilding())
                    {
                        towers.Add(grid[checkX, checkY].GetBuilding().GetComponent<BaseTower>());
                    }
                }
            }
            if (towers.Count == 0)
            {
                continue;
            }
            // If we found any towers, return the closest one, prefer Nexus if its in the list
            var nexus = Nexus.Instance.GetComponent<BaseTower>();
            if (towers.Contains(nexus))
            {
                return nexus;
            }
            else
            {
                return towers.OrderBy(t => Vector3.Distance(t.transform.position, new Vector3(x, y, 0))).First();
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        //Draw a grid
        Gizmos.color = Color.black;
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Gizmos.DrawWireCube(new Vector3(x + startX, y + startY, 0), Vector3.one);
            }
        }
    }




    public List<Tile> GetNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();
        int x = Mathf.RoundToInt(tile.transform.position.x - startX);
        int y = Mathf.RoundToInt(tile.transform.position.y - startY);
        if (x > 0)
        {
            neighbours.Add(grid[x - 1, y]);
        }
        if (x < WIDTH - 1)
        {
            neighbours.Add(grid[x + 1, y]);
        }
        if (y > 0)
        {
            neighbours.Add(grid[x, y - 1]);
        }
        if (y < HEIGHT - 1)
        {
            neighbours.Add(grid[x, y + 1]);
        }
        return neighbours;
    }




}
public enum TargetingType
{
    Ersten,
    Nächsten,
    Stärksten

}