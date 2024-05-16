using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attached to a tile of the grid. Holds information about the cost of the tile for the flow field.
/// </summary>
public class FlowFieldTile : MonoBehaviour
{
    [SerializeField]
    private int baseCost = 1;
    [SerializeField]
    private int assignedCost = 1;
    public Tile tile;
    [SerializeField]
    private Vector3 towerMovementVector;
    [SerializeField]
    private Vector3 nexusMovementVector;
    // Start is called before the first frame update
    void Start()
    {
        tile = GetComponent<Tile>();
    }
    public int GetBaseCost()
    {
        return baseCost;
    }
    public void SetAssignedCost(int cost)
    {
        assignedCost = cost;
    }
    public int GetAssignedCost()
    {
        return assignedCost;
    }

    public void SetNexusMovementVector(Vector3 vector)
    {
        nexusMovementVector = vector;
    }
    public Vector3 GetNexusMovementVector()
    {
        return nexusMovementVector;
    }
    public void SetTowerMovementVector(Vector3 vector)
    {
        towerMovementVector = vector;
    }
    public Vector3 GetTowerMovementVector()
    {
        return towerMovementVector;
    }



}
