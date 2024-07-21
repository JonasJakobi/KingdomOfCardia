using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;
using Unity.VisualScripting;

public class UpgradeUI : Singleton<UpgradeUI>
{

    public TMP_Text upgradeButton, towerName, health, damage, speed, range, projectile, defeatedEnemies, targetingType;
    public GameObject projectileBox;
    public GameObject rangeIndicatorPrefab;
    public GameObject rangeIndicator;
    public GameObject upgradeInfo;
    [SerializeField]
    public Tile selected;

    [SerializeField]
    private GameObject towerPlaceUI;



    void Update()
    {
        //Check if the mouse is over a ui element, in which case we dont want to select a tower
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Tile tile = GridManager.Instance.GetTileAtPosition(mousePos);

        //hovering over tower
        if (tile != null && tile.GetBuilding() != null && tile.GetBuilding().GetComponent<BaseTower>().isSelectable)
        {
            SpawnRangeIndicator(tile.GetBuilding().GetComponent<BaseTower>());

            if (Input.GetMouseButtonDown(0) && selected != tile)
            {
                //zuruecksetzen der alten groesse
                if (selected != null)
                {
                    selected.GetBuilding().GetComponent<BaseTower>().DeSelectTower();
                }

                //neue groesse
                tile.GetBuilding().GetComponent<BaseTower>().SelectTower();
                VisualiseUpgradeInfo(tile);
                upgradeInfo.SetActive(true);
                selected = tile;
                if (GameManager.Instance.State.Equals(GameState.BuildMode) || GameManager.Instance.State.Equals(GameState.Starting))
                {
                    Debug.Log("BuildMode");
                    towerPlaceUI.SetActive(false);

                }
            }
        } //hovering over empty tile
        else if (tile != null && tile.GetBuilding() == null)
        {
            if (selected == null)
            {
                DeleteRangeIndicator();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (GameManager.Instance.State.Equals(GameState.BuildMode) || GameManager.Instance.State.Equals(GameState.Starting))
                {
                    towerPlaceUI.SetActive(true);
                }
                if (selected != null)
                {
                    Unselect();

                }

            }
        }
        else if (selected == null)
        {
            upgradeInfo.SetActive(false);
        }
    }
    public void Unselect()
    {
        if (selected == null) return;
        selected.GetBuilding().GetComponent<BaseTower>().DeSelectTower();
        selected = null;
        upgradeInfo.SetActive(false);
        DeleteRangeIndicator();
        if (GameManager.Instance.State.Equals(GameState.BuildMode) || GameManager.Instance.State.Equals(GameState.Starting))
        {
            towerPlaceUI.SetActive(true);
        }
    }
    private void SpawnRangeIndicator(BaseTower tower)
    {
        var rangevalue = tower.GetComponent<BaseTower>().GetTowerUpgrade().range;
        DeleteRangeIndicator();
        rangeIndicator = Instantiate(rangeIndicatorPrefab, tower.transform.position, Quaternion.identity);
        //scale to range
        rangeIndicator.transform.localScale = new Vector3(rangevalue * 2 + 1, rangevalue * 2 + 1, 1);

    }
    private void DeleteRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            Destroy(rangeIndicator);

        }
    }

    public void VisualiseUpgradeInfo(Tile tile)
    {
        GameObject tower = tile.GetBuilding();
        towerName.text = tower.name.Replace("(Clone)", "");
        //Show tower range
        SpawnRangeIndicator(tower.GetComponent<BaseTower>());
        UpdateUpgradeButton(tower.GetComponent<BaseTower>());
        defeatedEnemies.text = "Defeated Enemies:\n" + tower.GetComponent<BaseTower>().GetEnemiesKilled().ToString();
        if (tower.GetComponent<ProjectileTower>() != null)
        {
            targetingType.transform.parent.gameObject.SetActive(true);
            targetingType.text = tower.GetComponent<ProjectileTower>().targetingType.ToString();
        }
        else
        {
            targetingType.transform.parent.gameObject.SetActive(false);
        }
        //Stats
        TowerUpgrade ctu = tower.GetComponent<BaseTower>().GetTowerUpgrade();
        TowerUpgrade ntu = tower.GetComponent<BaseTower>().GetTowerUpgrade(true);
        health.text = ctu.health.ToString() + "             " + ntu.health.ToString();
        damage.text = ctu.damage.ToString() + "             " + ntu.damage.ToString();
        range.text = ctu.range.ToString() + "             " + ntu.range.ToString();
        speed.text = (1 / ctu.attackSpeed).ToString("F2") + "             " + (1 / ntu.attackSpeed).ToString("F2");
        if (ctu.maxEnemiesHittable != 0)
        {
            projectileBox.SetActive(true);
            projectile.text = ctu.maxEnemiesHittable.ToString() + "             " + ntu.maxEnemiesHittable.ToString();
        }
        else
        {
            projectileBox.SetActive(false);
        }

    }
    public void UpdateUpgradeButton(BaseTower tower)
    {
        //Upgrade Button
        if (tower.GetComponent<BaseTower>().GetCostOfUpgrading() != -1)
        {
            upgradeButton.text = "Upgrade: " + tower.GetComponent<BaseTower>().GetCostOfUpgrading().ToString();
            upgradeButton.transform.parent.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            upgradeButton.text = "Fully Upgraded";
            upgradeButton.transform.parent.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    public void Upgrade()
    {
        selected.GetBuilding().GetComponent<BaseTower>().Upgrade();
        VisualiseUpgradeInfo(selected);

    }
    public void Sell()
    {
        selected.GetBuilding().GetComponent<BaseTower>().DestroyTower();
        selected = null;
        upgradeInfo.SetActive(false);
        if (GameManager.Instance.State.Equals(GameState.BuildMode) || GameManager.Instance.State.Equals(GameState.Starting))
        {
            towerPlaceUI.SetActive(true);
        }
    }
    public void NextTargetingType()
    {
        selected.GetBuilding().GetComponent<ProjectileTower>().NextTargetingType();
        VisualiseUpgradeInfo(selected);
    }
    public void PreviousTargetingType()
    {
        selected.GetBuilding().GetComponent<ProjectileTower>().PreviousTargetingType();
        VisualiseUpgradeInfo(selected);
    }

}
