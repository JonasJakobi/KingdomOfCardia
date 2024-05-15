using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
using UnityEditor.UIElements;
/// <summary>
/// Enable or disable debugging UI elements. And Control the modes for printing debug messages.
/// </summary>
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
        Enemies = 1 << 3,
        Towers = 1 << 4,
        // Add more modes here...
    }
    [Header("What kind of debug messages should be printed?")]

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
        return (activeConsoleDebugModes & mode) != 0;
    }
}
