using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleReseter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }
    private void OnEnable()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
