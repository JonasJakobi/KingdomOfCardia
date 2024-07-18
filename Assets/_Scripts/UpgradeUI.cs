using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UpgradeUI : MonoBehaviour
{

    public TMP_Text upgradeButton, towerName, health, damage, speed, range, DeleteButton;
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


        if (tile != null && tile.GetBuilding() != null && tile.GetBuilding().GetComponent<BaseTower>().isSelectable)
        {
            if (selected == null)
            {
                VisualiseUpgradeInfo(tile);
                upgradeInfo.SetActive(true);

            }

            if (Input.GetMouseButtonDown(0) && selected != tile)
            {
                //zur�cksetzen der alten gr��e
                if (selected != null)
                {
                    selected.GetBuilding().GetComponent<BaseTower>().DeSelectTower();
                }

                //neue gr��e
                tile.GetBuilding().GetComponent<BaseTower>().SelectTower();
                VisualiseUpgradeInfo(tile);
                selected = tile;
                if (GameManager.Instance.State.Equals(GameState.BuildMode) || GameManager.Instance.State.Equals(GameState.Starting))
                {
                    Debug.Log("BuildMode");
                    towerPlaceUI.SetActive(false);

                }
            }
        }
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
                    selected.GetBuilding().GetComponent<BaseTower>().DeSelectTower();
                    selected = null;
                    upgradeInfo.SetActive(false);

                }

            }
        }
        else if (selected == null)
        {
            upgradeInfo.SetActive(false);
        }
    }

    private void VisualiseUpgradeInfo(Tile tile)
    {
        GameObject tower = tile.GetBuilding();
        towerName.text = tower.name;
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
        speed.text = ctu.attackSpeed.ToString() + "             " + ntu.attackSpeed.ToString();
        //Delete Button

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
