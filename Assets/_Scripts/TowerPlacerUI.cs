using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

/// <summary>
/// Attached to sprite of a tower and allows the player to place towers on the grid. by dragging or clicking onto the grid.
/// </summary>
public class TowerPlacerUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    //temporary solution to tower costs
    [SerializeField]
    private int towerStartCost = 1;

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
    [SerializeField] private float towerCost = 1;
    [SerializeField] private int placedTowerAmount = 0;
    [SerializeField]
    private bool isPlacingTowerClick = false;
    [SerializeField]
    private bool isPlacingTowerDrag = false;
    [SerializeField]
    private TMP_Text money;

    void Start()
    {
        costText.text = GetTowerCostString();
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
        AudioSystem.Instance.PlayClickSound();
        if (MoneyManager.Instance.CanAfford(Convert.ToInt32(towerCost)))
        {
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
        MoneyManager.Instance.RemoveMoney(Convert.ToInt32(towerCost));
        Tile tile = GridManager.Instance.GetTileAtPosition(GetMousePosition());
        Instantiate(towerPrefab, tile.transform.position, Quaternion.identity);
        AudioSystem.Instance.PlayBonkSound();
        IncreaseCost();
    }

    private void IncreaseCost()
    {
        placedTowerAmount++;
        if (placedTowerAmount > 2)//starting from 3th tower, they get more and more expensive.
        {
            towerCost = towerCost * 1.3f;
        }


        costText.text = GetTowerCostString();
    }

    public string GetTowerCostString()
    {
        if (towerCost < 1000)
        {
            return Convert.ToInt32(towerCost).ToString();
        }
        else if (towerCost < 1000000)
        {
            return Convert.ToInt32((towerCost / 1000)).ToString(".0") + "k";
        }
        else
        {
            return Convert.ToInt32((towerCost / 1000000)).ToString(".0") + "M";
        }
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
