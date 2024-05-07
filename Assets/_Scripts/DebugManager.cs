using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{


    [ContextMenu("Test Method")]
    public void TestMethod()
    {
        Debug.Log("Test Method called");
    }
}
