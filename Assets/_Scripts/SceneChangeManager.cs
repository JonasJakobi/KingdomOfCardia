using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchSceneByName(string sceneName)
    {
        // Überprüfe, ob die Szene existiert und lade die Szene
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " cannot be loaded.");

        }
    }
}