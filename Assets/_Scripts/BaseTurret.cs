using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTurret : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        var tileHere = GridManager.Instance.GetTileAtPosition(transform.position);
        tileHere.SetHasBuilding(true);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        var tileHere = GridManager.Instance.GetTileAtPosition(transform.position);
        tileHere.SetHasBuilding(false);

    }
}
