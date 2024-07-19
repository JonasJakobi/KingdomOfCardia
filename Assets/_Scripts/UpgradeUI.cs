using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UpgradeUI : Singleton<UpgradeUI>
{

    public TMP_Text upgradeButton, towerName, health, damage, speed, range, projectile;
    public GameObject projectileBox;
    public GameObject upgradeInfo;
    [SerializeField]
    private Tile selected;

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
    }

    private void VisualiseUpgradeInfo(Tile tile)
    {
        GameObject tower = tile.GetBuilding();
        towerName.text = tower.name.Replace("(Clone)", "");
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

}
