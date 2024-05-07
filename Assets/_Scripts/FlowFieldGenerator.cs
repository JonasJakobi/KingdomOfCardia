using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class FlowFieldGenerator : MonoBehaviour
{

    List<FlowFieldTile> flowFieldTiles = new List<FlowFieldTile>();
    FlowFieldTile goalTile;
    // Start is called before the first frame update
    void Start()
    {
        //Initialize all tiles and the goal.
        FlowFieldTile[] tiles = FindObjectsOfType<FlowFieldTile>();
        foreach (FlowFieldTile tile in tiles)
        {
            flowFieldTiles.Add(tile);
        }
        goalTile = GridManager.Instance.GetTileAtPosition(GameObject.FindGameObjectWithTag("Goal").transform.position).GetComponent<FlowFieldTile>();

        GenerateFlowField();
        GenerateMovementVectors();
    }


    private void GenerateFlowField()
    {
        // Set the assigned cost of all tiles to 0
        foreach (FlowFieldTile tile in flowFieldTiles)
        {
            tile.SetAssignedCost(10000);
        }
        goalTile.SetAssignedCost(0);

        List<FlowFieldTile> openList = new List<FlowFieldTile>();
        List<FlowFieldTile> closedList = new List<FlowFieldTile>();

        openList.Add(goalTile);

        while (openList.Count > 0)
        {
            FlowFieldTile currentTile = openList[0];
            openList.Remove(currentTile);
            closedList.Add(currentTile);

            List<FlowFieldTile> neighbours = GetNeighbours(currentTile);
            foreach (FlowFieldTile neighbour in neighbours)
            {
                if (closedList.Contains(neighbour))
                {
                    continue;
                }
                int newCost = currentTile.GetAssignedCost() + neighbour.GetBaseCost();
                if (newCost < neighbour.GetAssignedCost())
                {
                    neighbour.SetAssignedCost(newCost);
                    neighbour.transform.GetChild(0).GetComponent<TextMeshPro>().text = newCost.ToString();
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
    }
    private void GenerateMovementVectors()
    {
        foreach (FlowFieldTile tile in flowFieldTiles)
        {
            List<FlowFieldTile> neighbours = GetNeighbours(tile);
            FlowFieldTile bestNeighbour = null;
            int bestCost = tile.GetAssignedCost();
            foreach (FlowFieldTile neighbour in neighbours)
            {
                if (neighbour.GetAssignedCost() < bestCost)
                {
                    bestCost = neighbour.GetAssignedCost();
                    bestNeighbour = neighbour;
                }
            }
            if (bestNeighbour != null)
            {
                Vector3 direction = bestNeighbour.transform.position - tile.transform.position;
                direction = direction.normalized;
                tile.tile.SetEnemyMovementVector(direction);
                tile.transform.GetChild(1).transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            }
            else
            {
                tile.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }


    private List<FlowFieldTile> GetNeighbours(FlowFieldTile tile)
    {
        if (tile == null)
        {
            Debug.LogError("Trying to get Neighbours: Tile is null though");
            return new List<FlowFieldTile>();
        }
        return GridManager.Instance.GetNeighbours(tile.tile).Select(t => t.GetComponent<FlowFieldTile>()).ToList();
    }



}
