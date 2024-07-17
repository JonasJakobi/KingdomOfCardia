using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeUI : MonoBehaviour
{

    public TMP_Text upgradeButton, towerName, health, damage, speed, range;
    public GameObject upgradeInfo;
    [SerializeField]
    private Tile selected;

    [SerializeField]
    private GameObject towerPlaceUI;



    void Update()
    {
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
                    Transform spriteTransform1 = selected.GetBuilding().transform;
                    spriteTransform1.localScale *= 0.66666667f;
                }

                //neue gr��e
                Transform spriteTransform = tile.GetBuilding().transform;
                spriteTransform.localScale *= 1.5f;
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
                    Transform spriteTransform1 = selected.GetBuilding().transform;
                    spriteTransform1.localScale *= 0.66666667f;
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
        upgradeButton.text = "Upgrade: " + tower.GetComponent<BaseTower>().GetCostOfUpgrading().ToString();

        TowerUpgrade ctu = tower.GetComponent<BaseTower>().GetTowerUpgrade();
        TowerUpgrade ntu = tower.GetComponent<BaseTower>().GetTowerUpgrade(true);
        health.text = ctu.health.ToString() + "             " + ntu.health.ToString();
        damage.text = ctu.damage.ToString() + "             " + ntu.damage.ToString();
        range.text = ctu.range.ToString() + "             " + ntu.range.ToString();
        speed.text = ctu.attackSpeed.ToString() + "             " + ntu.attackSpeed.ToString();
    }

    public void Upgrade()
    {
        selected.GetBuilding().GetComponent<BaseTower>().Upgrade();

    }

}
