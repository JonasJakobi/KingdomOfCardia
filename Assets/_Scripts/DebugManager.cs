using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
using UnityEditor.UIElements;

public class DebugManager : Singleton<DebugManager>
{
    HashSet<GameObject> debugObjects;
    [System.Flags]
    public enum DebugModes
    {
        None = 0,
        UI = 1 << 0,
        General = 1 << 1,
        GameFlow = 1 << 2,
        // Add more modes here...
    }

    [SerializeField]
    private DebugModes activeConsoleDebugModes = DebugModes.None;


    private void Start()
    {

        debugObjects = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("DebuggingUI"));
    }

    [ProButton]
    public void ShowOrHideDebugObjects()
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

    public bool IsDebugModeActive(DebugModes mode)
    {
        Debug.Log((activeConsoleDebugModes & mode) != 0);
        return (activeConsoleDebugModes & mode) != 0;
    }
}
