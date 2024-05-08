using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
/// <summary>
/// Basic GridManager to handle the grid of the game.
/// </summary>
public class GridManager : Singleton<GridManager>

{
    [SerializeField] private int startX, startY;
    public const int WIDTH = 16;
    public const int HEIGHT = 9;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Tile mountainTilePrefab;

    [SerializeField] private Transform camera;


    Tile[,] grid;
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
                //Small chance for mountain, otherwise normal tile
                if (Random.Range(0, 100) < 20)
                {
                    grid[x, y] = Instantiate(mountainTilePrefab, new Vector3(x + startX, y + startY, 0), Quaternion.identity).GetComponent<Tile>();
                    grid[x, y].name = $"Tile {x} {y}";
                    grid[x, y].transform.parent = transform;
                }
                else
                {
                    grid[x, y] = Instantiate(tilePrefab, new Vector3(x + startX, y + startY, 0), Quaternion.identity).GetComponent<Tile>();
                    grid[x, y].name = $"Tile {x} {y}";
                    grid[x, y].transform.parent = transform;
                }

            }
        }
        camera.position = new Vector3(WIDTH / 2, HEIGHT / 2, -10);

    }



    public Tile GetTileAtPosition(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x - startX));
        int y = Mathf.RoundToInt((pos.y - startY));

        if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT)
        {
            Debug.LogError("Tile out of bounds");
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
            Debug.LogError("Cant register enemy at " + x + y + " , Tile out of bounds");
            return;
        }
        {
            grid[x, y].RegisterEnemy(enemy);
        }

    }

    public Enemy FindEnemy(int x, int y, int maxDistance, TargetingType targetingType = TargetingType.Closest)
    {
        switch (targetingType)
        {
            case TargetingType.Closest:
                return FindClosestEnemy(x, y, maxDistance);
            case TargetingType.First:
                return FindFirstEnemy(x, y, maxDistance);
            case TargetingType.Strongest:
                return FindStrongestEnemy(x, y, maxDistance);
            default:
                return null;
        }
    }
    /// <summary>
    /// Find the enemy closest to the (x,y) position within a maxDistance
    /// !! Current implementation doesnt always get the first one, as it gets the closest one, from the first tile it finds with enemies.
    /// If we have no problems with performance, we can just loop through all tiles and remember the closest enemy.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    public Enemy FindClosestEnemy(int x, int y, int maxDistance)
    {
        for (int d = 0; d < maxDistance; d++)
        {
            for (int i = -d; i <= d; i++)
            {
                for (int j = -d; j <= d; j++)
                {
                    if (i != -d && i != d && j != -d && j != d) continue; // Skip tiles inside the square
                    int checkX = x + i;
                    int checkY = y + j;
                    if (checkX < 0 || checkX >= WIDTH || checkY < 0 || checkY >= HEIGHT) continue; // Skip out-of-bounds tiles
                    if (grid[checkX, checkY].enemies.Count > 0)
                    {
                        //Find closest enemy within tile
                        Enemy closestEnemy = null;
                        float shortestDistance = float.MaxValue;

                        foreach (var enemy in grid[checkX, checkY].enemies)
                        {
                            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(enemy.transform.position.x, enemy.transform.position.y));
                            if (distance < shortestDistance)
                            {
                                shortestDistance = distance;
                                closestEnemy = enemy;
                            }
                        }
                        return closestEnemy;
                    }
                }
            }
        }
        return null;
    }
    /// <summary>
    /// Find the enemy closest to the Nexus but within a maxDistance of the (x,y) position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    public Enemy FindFirstEnemy(int x, int y, int maxDistance)
    {
        Vector3 nexusPosition = Nexus.Instance.transform.position;
        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;

        for (int d = 0; d < maxDistance; d++)
        {
            for (int i = -d; i <= d; i++)
            {
                for (int j = -d; j <= d; j++)
                {
                    if (i != -d && i != d && j != -d && j != d) continue; // Skip tiles inside the square
                    int checkX = x + i;
                    int checkY = y + j;
                    if (checkX < 0 || checkX >= WIDTH || checkY < 0 || checkY >= HEIGHT) continue; // Skip out-of-bounds tiles

                    Tile tile = grid[checkX, checkY];
                    foreach (Enemy enemy in tile.enemies)
                    {
                        float distance = Vector3.Distance(enemy.transform.position, nexusPosition);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = enemy;
                        }
                    }
                }
            }
        }

        return closestEnemy;
    }
    /// <summary>
    /// Find the enemy with the highest health within a maxDistance of the (x,y) position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    public Enemy FindStrongestEnemy(int x, int y, int maxDistance)
    {
        float highestHealth = 0;
        Enemy strongestEnemy = null;

        for (int d = 0; d < maxDistance; d++)
        {
            for (int i = -d; i <= d; i++)
            {
                for (int j = -d; j <= d; j++)
                {
                    if (i != -d && i != d && j != -d && j != d) continue; // Skip tiles inside the square
                    int checkX = x + i;
                    int checkY = y + j;
                    if (checkX < 0 || checkX >= WIDTH || checkY < 0 || checkY >= HEIGHT) continue; // Skip out-of-bounds tiles
                    if (grid[checkX, checkY].enemies.Count > 0)
                    {
                        foreach (var enemy in grid[checkX, checkY].enemies)
                        {
                            if (enemy.health > highestHealth)
                            {
                                highestHealth = enemy.health;
                                strongestEnemy = enemy;
                            }
                        }
                    }
                }
            }
        }
        return strongestEnemy;
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
    First,
    Closest,
    Strongest
}