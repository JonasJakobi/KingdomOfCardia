using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// Basic GridManager to handle the grid of the game.
/// </summary>
public class GridManager : Singleton<GridManager>

{
    [SerializeField] private int startX, startY;
    [SerializeField] private int width, height;
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
        grid = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Small chance for mountain, otherwise normal tile
                if (Random.Range(0, 100) < 5)
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
        camera.position = new Vector3(width / 2, height / 2, -10);

    }



    public Tile GetTileAtPosition(Vector3 pos)
    {
        int x = Mathf.FloorToInt((pos.x - startX));
        int y = Mathf.FloorToInt((pos.y - startY));

        if (x < 0 || x >= width || y < 0 || y >= height)
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

    public Vector3 GetCenterOfTile(Tile tile)
    {
        return new Vector3(tile.transform.position.x + 0.5f, tile.transform.position.y + 0.5f, 0);
    }


    public void UnregisterEnemyAtTile(Enemy enemy, int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
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
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            Debug.LogError("Cant register enemy at " + x + y + " , Tile out of bounds");
            return;
        }
        {
            grid[x, y].RegisterEnemy(enemy);
        }

    }

    public Enemy FindClosestEnemy(int x, int y)
    {
        int maxDistance = Mathf.Max(width, height);
        for (int d = 0; d < maxDistance; d++)
        {
            for (int i = -d; i <= d; i++)
            {
                for (int j = -d; j <= d; j++)
                {
                    if (i != -d && i != d && j != -d && j != d) continue; // Skip tiles inside the square
                    int checkX = x + i;
                    int checkY = y + j;
                    if (checkX < 0 || checkX >= width || checkY < 0 || checkY >= height) continue; // Skip out-of-bounds tiles
                    if (grid[checkX, checkY].enemies.Count > 0)
                    {
                        // Return the closest enemy in that tile.
                        return grid[checkX, checkY].enemies.OrderBy(enemy => Vector2.Distance(new Vector2(x, y), new Vector2(enemy.transform.position.x, enemy.transform.position.y))).FirstOrDefault();
                    }
                }
            }
        }
        return null;
    }

    public List<Tile> GetNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();
        int x = Mathf.FloorToInt(tile.transform.position.x - startX);
        int y = Mathf.FloorToInt(tile.transform.position.y - startY);
        if (x > 0)
        {
            neighbours.Add(grid[x - 1, y]);
        }
        if (x < width - 1)
        {
            neighbours.Add(grid[x + 1, y]);
        }
        if (y > 0)
        {
            neighbours.Add(grid[x, y - 1]);
        }
        if (y < height - 1)
        {
            neighbours.Add(grid[x, y + 1]);
        }
        return neighbours;
    }


}