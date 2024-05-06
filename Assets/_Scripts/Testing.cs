using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public Enemy testEnemy;


    private void Start()
    {
        //Move enemy to the right
        testEnemy.MoveTowards(new Vector3(5, 4, 0), 1);



    }
    private void Update()
    {
        testEnemy.MoveTowards(new Vector3(5, 4, 0), 1);
        Debug.Log(GridManager.Instance.FindClosestEnemy(0, 0));
    }
}
