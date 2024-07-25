using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Unity.VisualScripting;

/// <summary>
/// Attached to sprite of a tower and allows the player to place towers on the grid. by dragging or clicking onto the grid.
/// </summary>
public class TowerPlacerUI : MonoBehaviour
{
    //temporary solution to tower costs
    [SerializeField]
    private int towerStartCost = 1;

    [SerializeField]
    GameObject RangeIndicatorPrefab;
    [SerializeField]
    GameObject towerPreviewPrefab;
    GameObject rangeIndicator;
    GameObject towerPreview;
    [SerializeField]
    public GameObject towerPrefab;

    [SerializeField]
    private Color placeableColor = new Color(0, 1, 0, 0.5f);
    [SerializeField]
    private Color notPlaceableColor = new Color(1, 0, 0, 0.5f);

    [SerializeField]
    private TextMeshProUGUI costText;
    [Header("Visible for debugging:")]
    [SerializeField] private float towerCost = 1;
    [SerializeField] public int placedTowerAmount = 0;
    [SerializeField]
    private bool isPlacingTower = false;
    [SerializeField]
    private TMP_Text money;
    [SerializeField] private GameObject placePrefab;

    public KeyCode placeThisTowerKey;
    void Start()
    {
        costText.text = GetTowerCostString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(placeThisTowerKey))
        {
            ButtonClick();
        }
        //Catch trying to place another tower type or open menu
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            StopPlacingTower();
        }


        if (!isPlacingTower)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
            OnMouseDown();

        Vector3 mousePos = GetMousePosition();
        Tile tile = GridManager.Instance.GetTileAtPosition(mousePos);
        if (tile == null)
        {
            if (towerPreview != null)
            {
                towerPreview.SetActive(false);
                rangeIndicator.SetActive(false);

            }

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
        if (isPlacingTower)
        {
            StopPlacingTower();
            return;
        }
        AudioSystem.Instance.PlayClickSound();

        if (MoneyManager.Instance.CanAfford(Convert.ToInt32(towerCost)))
        {
            isPlacingTower = true;
            towerPreview = Instantiate(towerPreviewPrefab, GetMousePosition(), Quaternion.identity);
            rangeIndicator = Instantiate(RangeIndicatorPrefab, GetMousePosition(), Quaternion.identity);
            rangeIndicator.transform.localScale = new Vector3(towerPrefab.GetComponent<BaseTower>().GetTowerUpgrade().range * 2, towerPrefab.GetComponent<BaseTower>().GetTowerUpgrade().range * 2, 1);



        }
        else
        {
            StartCoroutine(MoneyUIRed());
        }

    }

    private IEnumerator MoneyUIRed()
    {
        money.color = Color.red;
        yield return new WaitForSeconds(1.5f);
        money.color = Color.white;
    }



    private void UpdatePreview(Vector3 mousePos, Tile tile, bool isBuildable)
    {
        if (towerPreview.activeSelf == false)
        {
            towerPreview.SetActive(true);
            rangeIndicator.SetActive(true);
        }

        if (towerPreview.GetComponent<SpriteRenderer>() != null)
        {
            towerPreview.GetComponent<SpriteRenderer>().color = isBuildable ? placeableColor : notPlaceableColor;
        }
        else
        {
            towerPreview.GetComponentInChildren<SpriteRenderer>().color = isBuildable ? placeableColor : notPlaceableColor;
        }


        towerPreview.transform.position = tile.transform.position;
        rangeIndicator.transform.position = tile.transform.position;
    }



    private void PlaceTower()
    {
        MoneyManager.Instance.RemoveMoney(Convert.ToInt32(towerCost));
        Tile tile = GridManager.Instance.GetTileAtPosition(GetMousePosition());
        var tower = Instantiate(towerPrefab, tile.transform.position, Quaternion.identity);
        //give the tower our decrease cost method via event or something
        if (tower.GetComponent<BaseTower>() == null)
        {
            tower.GetComponentInChildren<BaseTower>().OnTowerDestroyed += DecreaseCost;
        }
        else
        {
            tower.GetComponent<BaseTower>().OnTowerDestroyed += DecreaseCost;
        }

        AudioSystem.Instance.PlayBonkSound();
        IncreaseCost();
        UIChangeManager.Instance.towersPlaced++;

    }

    private void IncreaseCost()
    {
        placedTowerAmount++;
        if (placedTowerAmount > 2)//starting from 3th tower, they get more and more expensive.
        {
            towerCost = towerCost * Constants.Instance.towerCostGrowthRate;
        }


        costText.text = GetTowerCostString();
    }
    /// <summary>
    /// Called when a tower gets destroyed
    /// </summary>
    private void DecreaseCost()
    {
        placedTowerAmount--;
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddMoney(Convert.ToInt32(towerCost / 2)); // refund half of the cost
        }
        if (placedTowerAmount > 1)//starting from 3th tower, they get more and more expensive.
        {
            towerCost = towerCost / Constants.Instance.towerCostGrowthRate;
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
        return isPlacingTower && Input.GetMouseButtonDown(0);
    }

    private void StopPlacingTower()
    {
        isPlacingTower = false;
        if (towerPreview != null)
        {
            Destroy(towerPreview);
            Destroy(rangeIndicator);
        }

    }

    private void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            StopPlacingTower();
            return;
        }
        if (!isPlacingTower)
        {
            return;

        }


        var placeAnim = Instantiate(placePrefab, transform);
        Destroy(placeAnim, 1);

        Vector3 mousePos = GetMousePosition();
        Tile tile = GridManager.Instance.GetTileAtPosition(mousePos);
        //If we are not hovering over a tile, we let go of the mouse on the UI so we placing tower with click
        StopPlacingTower();
        if (tile != null && tile.IsBuildable())
        {
            PlaceTower();
        }

    }



}
