using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Tile of the grid. 
/// Instantiated by the GridManager from the Tile prefab.
/// Holds information about walkability and towers and enemies on it.
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, hoverColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    /// <summary>
    /// Enemies can (theoretically) walk on this tile.
    /// Use the public method IsWalkable() to check if the tile is currently walkable.
    /// </summary>
    [SerializeField] private bool isWalkable;
    /// <summary>
    /// Towers can (theoretically) be built on this tile.
    /// Use the public method IsBuildable() to check if the tile is currently buildable.
    /// </summary>
    [SerializeField] private bool isBuildable;

    [SerializeField] private bool hasBuilding;
    [SerializeField] private GameObject building;
    [SerializeField]
    public List<Enemy> enemies = new List<Enemy>();
    public FlowFieldTile flowFieldTile;
    public TileType Type;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = baseColor;
        flowFieldTile = GetComponent<FlowFieldTile>();

    }

    public enum TileType
    {
        Grass,
        Mountain,
        Forest
    }

    public bool IsWalkable(bool ignoreBuilding = false)
    {
        if (isBuildable && hasBuilding && !ignoreBuilding)
        {
            return false;
        }
        return isWalkable;
    }
    public void SetIsWalkable(bool value)
    {
        isWalkable = value;
        FlowFieldGenerator.Instance.RequestFlowFieldRecalculation();
    }

    public bool IsBuildable()
    {
        return isBuildable && !hasBuilding;
    }


    public void RegisterEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void SetHasBuilding(bool value, GameObject building)
    {
        Debug.Log("Set has building called!");
        hasBuilding = value;
        if (value)
        {
            this.building = building;
        }
        else
        {
            this.building = null;
        }
        FlowFieldGenerator.Instance.RequestFlowFieldRecalculation();
    }
    public bool HasBuilding()
    {
        return hasBuilding;

    }
    /// <summary>
    /// Get a reference to the building gameobject on this tile.
    /// </summary>
    /// <returns></returns>
    public GameObject GetBuilding()
    {
        return building;
    }
    public void SetIsBuildable(bool buildable)
    {
        isBuildable = buildable;
    }



}
