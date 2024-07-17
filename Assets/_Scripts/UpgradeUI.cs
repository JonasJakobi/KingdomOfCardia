using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeUI : MonoBehaviour
{

    public TMP_Text upgradeButton, towerName, health, damage, speed, range;
    public GameObject upgradeInfo;

    private Tile selected;

    void Start()
    {
        
    }
    
    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Tile tile = GridManager.Instance.GetTileAtPosition(mousePos);
        if (tile != null && tile.GetBuilding() != null)
        {
            if(selected ==null)
            {
                VisualiseUpgradeInfo(tile);
                upgradeInfo.SetActive(true);
            }

            if (Input.GetMouseButtonDown(0) && selected != tile)
            {
                //zurücksetzen der alten größe
                if(selected != null)
                {
                    Transform spriteTransform1 = selected.GetBuilding().transform;
                    spriteTransform1.localScale *= 0.666667f;
                }

                //neue größe
                Transform spriteTransform = tile.GetBuilding().transform;
                spriteTransform.localScale *= 1.5f;
                VisualiseUpgradeInfo(tile);
                selected = tile;
            }
        }
        else if(selected ==null)
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
        Transform spriteTransform1 = selected.GetBuilding().transform;
        spriteTransform1.localScale *= 0.75f;
        selected = null;
    }
}
