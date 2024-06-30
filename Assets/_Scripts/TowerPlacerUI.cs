using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Attached to sprite of a tower and allows the player to place towers on the grid. by dragging or clicking onto the grid.
/// </summary>
public class TowerPlacerUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    //temporary solution to tower costs
    [SerializeField]
    private int towerCost = 1;
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

    [SerializeField]
    private TextMeshProUGUI costText;
    [Header("Visible for debugging:")]
    [SerializeField]
    private bool isPlacingTowerClick = false;
    [SerializeField]
    private bool isPlacingTowerDrag = false;
    [SerializeField]
    private TMP_Text money;

    void Start()
    {
        costText.text = towerCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        if (!(isPlacingTowerClick || isPlacingTowerDrag) || stillOnUI)
        {
            return;
        }
        if (Input.GetMouseButtonUp(0))
            OnMouseUp();

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

        if (towerPreview != null)
            UpdatePreview(mousePos, tile, isBuildable);

        //we try to place the tower but we cant:
        if (!isBuildable && UserTryingToPlace())
        {
            if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
            {
                Debug.Log("Trying to place tower but we cant");
            }
            StopPlacingTower();
            MoneyManager.Instance.RemoveMoney(towerCost);
        }
        //We place the tower:
        else if (UserTryingToPlace())
        {
            PlaceTower();
            StopPlacingTower();
        }

    }


    public void ButtonClick()
    {
        Debug.Log("Clicked on tower placer");
        AudioSystem.Instance.PlayClickSound();
        print(MoneyManager.Instance.CanAfford(towerCost));
        if (MoneyManager.Instance.CanAfford(towerCost))
        {
            MoneyManager.Instance.RemoveMoney(towerCost);
            isPlacingTowerDrag = true;
            money.color = Color.white;
        }
        else
        {
            money.color = Color.red;
        }

    }



    private void UpdatePreview(Vector3 mousePos, Tile tile, bool isBuildable)
    {
        Debug.Log("In update");
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
        Instantiate(towerPrefab, tile.transform.position, Quaternion.identity);
        AudioSystem.Instance.PlayBonkSound();
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

    private void OnMouseUp()
    {
        Debug.Log("Mouse up");
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
                MoneyManager.Instance.AddMoney(towerCost);
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

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("onmouseexit");
        stillOnUI = false;
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
        {
            Debug.Log("Mouse left tower placer");
        }
        if (isPlacingTowerDrag || isPlacingTowerClick)
        {
            Debug.Log("Spawn towerpreview");
            towerPreview = Instantiate(towerPreviewPrefab, GetMousePosition(), Quaternion.identity);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        stillOnUI = true;
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.UI))
        {
            Debug.Log("Mouse entered tower placer");
        }
        StopPlacingTower();
    }
}
