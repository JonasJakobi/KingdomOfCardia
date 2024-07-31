using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class FlowFieldGenerator : Singleton<FlowFieldGenerator>
{
    List<FlowFieldTile> flowFieldTiles = new List<FlowFieldTile>();
    FlowFieldTile goalTile;
    // Start is called before the first frame update


    public void InitializeFlowField(List<FlowFieldTile> tiles)
    {
        flowFieldTiles = tiles;
        GenerateFlowField(false);
        GenerateFlowField(true);
    }


    private void GenerateFlowField(bool toGoal = true)
    {
        if (GridManager.Instance == null || GameObject.FindGameObjectWithTag("Goal") == null) // exiting playmode
        {
            return;
        }
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.General))
        {
            Debug.Log("Regenerating Flow Field...");
        }
        goalTile = GridManager.Instance.GetTileAtPosition(GameObject.FindGameObjectWithTag("Goal").transform.position).GetComponent<FlowFieldTile>();
        List<FlowFieldTile> openList = new List<FlowFieldTile>();
        List<FlowFieldTile> closedList = new List<FlowFieldTile>();



        // Set the assigned cost of all tiles to 0
        foreach (FlowFieldTile tile in flowFieldTiles)
        {
            SetTileVector(tile, new Vector3(-1, -1, -1), toGoal);
            tile.SetAssignedCost(10000);
            if (!toGoal && tile.tile.HasBuilding())
            {
                tile.SetAssignedCost(0);
                openList.Add(tile);
            }
        }
        goalTile.SetAssignedCost(0);
        openList.Add(goalTile);

        while (openList.Count > 0)
        {
            FlowFieldTile currentTile = openList[0];
            openList.Remove(currentTile);
            closedList.Add(currentTile);

            List<FlowFieldTile> neighbours = GetWalkableNeighbours(currentTile, !toGoal);
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
                    if (!openList.Contains(neighbour) && !neighbour.tile.HasBuilding())
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        GenerateMovementVectors(toGoal);
    }
    private void GenerateMovementVectors(bool toGoal = true)
    {
        foreach (FlowFieldTile tile in flowFieldTiles)
        {
            List<FlowFieldTile> neighbours = GetWalkableNeighbours(tile, false);
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
                Vector3 direction = bestNeighbour.transform.position;
                SetTileVector(tile, direction, toGoal);
            }
            else
            {
                SetTileVector(tile, new Vector3(-1, -1, -1), toGoal);
                // If there is no best neighbour, the tile is the goal tile
                tile.transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        foreach (FlowFieldTile tile in flowFieldTiles)
        {
            if (!tile.tile.IsWalkable())
            {
                SetTileVector(tile, new Vector3(-1, -1, -1), toGoal);
            }
        }
    }

    private void SetTileVector(FlowFieldTile tile, Vector3 vector, bool toGoal)
    {
        if (toGoal)
        {
            tile.SetNexusMovementVector(vector);
        }
        else
        {
            tile.SetTowerMovementVector(vector);
        }
    }
    private List<FlowFieldTile> GetWalkableNeighbours(FlowFieldTile tile, bool ignoreBuilding = false)
    {
        if (tile == null)
        {
            Debug.LogError("Trying to get Neighbours: Tile is null though");
            return new List<FlowFieldTile>();
        }
        //Return the FlowField Component of all walkable neighbours
        return GridManager.Instance.GetNeighbours(tile.tile)
            .Where(t => t.IsWalkable(ignoreBuilding))
            .Select(t => t.GetComponent<FlowFieldTile>())
            .ToList();
    }

    public void RequestFlowFieldRecalculation()
    {

        GenerateFlowField(false);
        GenerateFlowField(true);

    }



}
