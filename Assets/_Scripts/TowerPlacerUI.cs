using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacerUI : MonoBehaviour
{
    private bool stillOnUI = false;

    [SerializeField]
    GameObject towerPreviewPrefab;

    GameObject towerPreview;
    [SerializeField]
    GameObject towerPrefab;

    [SerializeField]
    private Color placeableColor = new Color(0, 1, 0, 0.5f);
    [SerializeField]
    private Color notPlaceableColor = new Color(1, 0, 0, 0.5f);
    [Header("Visible for debugging:")]
    [SerializeField]
    private bool isPlacingTowerClick = false;
    [SerializeField]
    private bool isPlacingTowerDrag = false;

    // Update is called once per frame
    void Update()
    {

        if (!(isPlacingTowerClick || isPlacingTowerDrag) || stillOnUI)
        {
            return;
        }

        Vector3 mousePos = GetMousePosition();
        Tile tile = GridManager.Instance.GetTileAtPosition(mousePos);
        if (tile == null)
        {
            towerPreview.SetActive(false);
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
            {
                Debug.Log("Currently not hovering over a tile so we are not doing anything here");
            }
            return;
        }

        bool isBuildable = !tile.HasBuilding() && tile.IsBuildable();

        UpdatePreview(mousePos, tile, isBuildable);

        //we try to place the tower but we cant:
        if (!isBuildable && UserTryingToPlace())
        {
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
            {
                Debug.Log("Trying to place tower but we cant");
            }
            StopPlacingTower();
        }
        //We place the tower:
        else if (UserTryingToPlace())
        {
            PlaceTower();
            StopPlacingTower();
        }

    }

    private void UpdatePreview(Vector3 mousePos, Tile tile, bool isBuildable)
    {
        if (towerPreview.activeSelf == false)
        {
            towerPreview.SetActive(true);
        }
        towerPreview.transform.position = tile.transform.position;

        towerPreview.GetComponent<SpriteRenderer>().color = isBuildable ? placeableColor : notPlaceableColor;
        towerPreview.transform.position = tile.transform.position;
    }



    private void PlaceTower()
    {
        Tile tile = GridManager.Instance.GetTileAtPosition(GetMousePosition());
        tile.SetHasBuilding(true);
        Instantiate(towerPrefab, tile.transform.position, Quaternion.identity);

    }




    private Vector3 GetMousePosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
    private bool UserTryingToPlace()
    {
        return isPlacingTowerClick && Input.GetMouseButtonDown(0);
    }

    private void StopPlacingTower()
    {
        isPlacingTowerClick = false;
        isPlacingTowerDrag = false;
        if (towerPreview != null)
        {
            Destroy(towerPreview);
        }

    }
    //The following methods check behavior around user input on the UI
    private void OnMouseDown()
    {
        Debug.Log("Clicked on tower placer");
        isPlacingTowerDrag = true;
    }
    private void OnMouseUp()
    {

        if (!isPlacingTowerDrag)
        {
            return;

        }

        Vector3 mousePos = GetMousePosition();
        Tile tile = GridManager.Instance.GetTileAtPosition(mousePos);
        //If we are not hovering over a tile, we let go of the mouse on the UI so we placing tower with click
        if (tile == null)
        {
            if (stillOnUI)
            {
                isPlacingTowerDrag = false;
                isPlacingTowerClick = true;
                if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
                {
                    Debug.Log("Placing Tower with Click now");
                }
            }
            //not hovering over the UI but also not on tile, so we stop placing tower
            else
            {
                StopPlacingTower();
            }

        }
        else
        {
            if (tile.IsBuildable())
            {
                PlaceTower();
            }
            StopPlacingTower();
        }

    }

    private void OnMouseExit()
    {
        stillOnUI = false;
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
        {
            Debug.Log("Mouse left tower placer");
        }
        if (isPlacingTowerDrag || isPlacingTowerClick)
        {
            towerPreview = Instantiate(towerPreviewPrefab, GetMousePosition(), Quaternion.identity);
        }
    }
    private void OnMouseEnter()
    {
        stillOnUI = true;
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
        {
            Debug.Log("Mouse entered tower placer");
        }
        StopPlacingTower();
    }
}
