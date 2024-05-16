using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
using UnityEditor.UIElements;
using System.Runtime.InteropServices;
/// <summary>
/// Enable or disable debugging UI elements. And Control the modes for printing debug messages.
/// </summary>
public class DebugManager : Singleton<DebugManager>
{
    [Header("References")]
    [SerializeField] GameObject arrowPrefab;

    HashSet<GameObject> debugObjects;

    [System.Flags]
    public enum DebugModes
    {
        None = 0,
        UI = 1 << 0,
        General = 1 << 1,
        GameFlow = 1 << 2,
        Enemies = 1 << 3,
        Towers = 1 << 4,
        // Add more modes here...
    }
    [Header("What kind of debug messages should be printed?")]

    [SerializeField]
    private DebugModes activeConsoleDebugModes = DebugModes.None;

    private bool showingNexusArrows = false;
    private bool showingTowerArrows = false;


    List<GameObject> allArrows = new List<GameObject>();
    private void Start()
    {

        debugObjects = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("DebuggingUI"));
    }

    [ProButton]
    public void ShowOrHideDebugObjects()
    {
        var allCurrent = GameObject.FindGameObjectsWithTag("DebuggingUI");

        debugObjects.UnionWith(allCurrent);
        foreach (GameObject obj in debugObjects)
        {
            if (obj == null)
            {
                continue;
            }
            obj.SetActive(!obj.activeSelf);
        }
    }

    public bool IsDebugModeActive(DebugModes mode)
    {
        return (activeConsoleDebugModes & mode) != 0;
    }

    [ProButton]
    public void ShowNexusArrows()
    {
        if (showingTowerArrows)
        {
            showingTowerArrows = false;
            allArrows.ForEach(Destroy);

        }
        if (showingNexusArrows)
        {
            showingNexusArrows = false;
            allArrows.ForEach(Destroy);
            return;
        }
        var allTiles = FindObjectsOfType<FlowFieldTile>();
        foreach (FlowFieldTile tile in allTiles)
        {
            if (tile.GetNexusMovementVector() != Vector3.zero)
            {
                GameObject arrow = Instantiate(arrowPrefab, tile.transform.position, Quaternion.identity);
                allArrows.Add(arrow);
                arrow.transform.up = tile.GetNexusMovementVector();
            }
        }
        showingNexusArrows = true;
    }
    [ProButton]
    public void ShowTowerArrows()
    {
        if (showingNexusArrows)
        {
            showingNexusArrows = false;
            allArrows.ForEach(Destroy);
        }
        if (showingTowerArrows)
        {
            showingTowerArrows = false;
            allArrows.ForEach(Destroy);
            return;
        }
        var allTiles = FindObjectsOfType<FlowFieldTile>();
        foreach (FlowFieldTile tile in allTiles)
        {
            if (tile.GetTowerMovementVector() != Vector3.zero)
            {
                GameObject arrow = Instantiate(arrowPrefab, tile.transform.position, Quaternion.identity);
                allArrows.Add(arrow);
                arrow.transform.up = tile.GetTowerMovementVector().normalized;
            }
        }
        showingTowerArrows = true;
    }

}
