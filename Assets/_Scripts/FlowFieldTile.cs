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
    private int assignedCost = 1;
    public Tile tile;
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


}
