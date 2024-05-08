using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacerUI : MonoBehaviour
{
    private bool isPlacingTowerClick = false;
    private bool isPlacingTowerDrag = false;
    [SerializeField]
    GameObject towerPreviewPrefab;

    GameObject towerPreview;
    [SerializeField]
    GameObject towerPrefab;

    [SerializeField]
    private Color placeableColor = new Color(0, 1, 0, 0.5f);
    [SerializeField]
    private Color notPlaceableColor = new Color(1, 0, 0, 0.5f);

    // Update is called once per frame
    void Update()
    {

        if (!(isPlacingTowerClick || isPlacingTowerDrag))
        {
            return;
        }
        //from this point on user is still in the process of placing a tower

        Vector3 mousePos = GetMousePosition();
        Tile tile = GridManager.Instance.GetTileAtPosition(mousePos);
        towerPreview.transform.position = tile.transform.position;
        bool isBuildable = !tile.HasBuilding() && tile.IsBuildable();
        towerPreview.GetComponent<SpriteRenderer>().color = isBuildable ? placeableColor : notPlaceableColor;
        towerPreview.transform.position = tile.transform.position;
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
        }

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
        return (isPlacingTowerClick && Input.GetMouseButtonDown(0)) || (isPlacingTowerDrag && Input.GetMouseButtonUp(0));
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
        //If the user clicked and released the mouse button, we are placing a tower
        if (isPlacingTowerDrag)
        {
            isPlacingTowerDrag = false;
            isPlacingTowerClick = true;
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
            {
                Debug.Log("Placing Tower with Click now");
            }
        }
    }
    private void OnMouseExit()
    {
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
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
        {
            Debug.Log("Mouse entered tower placer");
        }
        StopPlacingTower();
    }
}
