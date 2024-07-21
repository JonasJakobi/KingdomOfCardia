using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;

using System.Runtime.InteropServices;
using Unity.VisualScripting;
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
    private void Update()
    {
        HandleDebugInputs();
    }

    private void HandleDebugInputs()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ShowOrHideDebugObjects();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tile = GridManager.Instance.GetTileAtPosition(mousePos);
            if (tile != null && tile.GetBuilding() != null)
            {
                Debug.Log("Tile at " + tile.transform.position + " has " + tile.GetBuilding().name + " as building.");
                tile.GetBuilding().GetComponent<BaseTower>().Upgrade();
                UpgradeUI.Instance.UpdateUpgradeButton(tile.GetBuilding().GetComponent<BaseTower>());
            }
        }
        if (Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Y))
        {
            MoneyManager.Instance.AddMoney(1);
            MoneyManager.Instance.AddMoney(Mathf.RoundToInt(MoneyManager.Instance.money * Time.deltaTime * 1.0f));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tile = GridManager.Instance.GetTileAtPosition(mousePos);
            if (tile == null) return;

            if (tile.GetBuilding() != null && tile.GetBuilding().GetComponent<Nexus>() == null)
            {
                if (UpgradeUI.Instance.selected == tile)
                {
                    UpgradeUI.Instance.Unselect();
                }
                tile.GetBuilding().GetComponent<BaseTower>().DestroyTower();

            }
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tile = GridManager.Instance.GetTileAtPosition(mousePos);
            if (tile == null) return;

            if (tile.GetBuilding() != null && tile.GetBuilding().GetComponent<Nexus>() == null)
            {
                tile.GetBuilding().GetComponent<BaseTower>().TakeDamage(1000000);
            }
            if (tile.enemies.Count > 0)
            {
                tile.enemies.ForEach(e => e.TakeDamage(1000000));
            }

        }
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
                arrow.transform.up = (tile.GetNexusMovementVector() - tile.transform.position).normalized;
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
                arrow.transform.up = (tile.GetTowerMovementVector() - tile.transform.position).normalized;
            }
        }
        showingTowerArrows = true;
    }
    [ProButton]
    public void SpawnEnemy(GameObject prefab, int x, int y, int amount = 1)
    {
        var tile = GridManager.Instance.GetTileAtPosition(new Vector3(x, y, 0));
        if (tile == null)
        {
            Debug.LogError("Tile at " + x + ", " + y + " does not exist.");
            return;
        }
        // Always spawn at least one enemy
        if (amount == 0)
        {
            amount = 1;
        }
        // Spawn the enemies with slight delays to see them better
        for (int i = 0; i < amount; i++)
        {
            var enemy = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

}
