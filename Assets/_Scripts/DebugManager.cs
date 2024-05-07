using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;

public class DebugManager : MonoBehaviour
{
    HashSet<GameObject> debugObjects;
    private void Start()
    {

        debugObjects = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("DebuggingUI"));
    }

    [ProButton]
    public void ToggleDebuggingUIObjects()
    {
        var allCurrent = GameObject.FindGameObjectsWithTag("DebuggingUI");

        debugObjects.UnionWith(allCurrent);
        foreach (GameObject obj in debugObjects)
        {
            if (obj == null)
            {
                continue;
            }
            obj.SetActive(!obj.activeSelf);
        }
    }
}
