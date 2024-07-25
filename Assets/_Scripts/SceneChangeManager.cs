using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : SingletonPersistent<SceneChangeManager>
{
    public bool SwitchingScene = false;
    // Start is called before the first frame update
    void Start()
    {
        SwitchingScene = false;
    }
    private void OnEnable()
    {
        SwitchingScene = false;
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchSceneByName(string sceneName)
    {
        SwitchingScene = true;
        // Überprüfe, ob die Szene existiert und lade die Szene
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            SwitchingScene = false;
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " cannot be loaded.");

        }
    }

    public void EndGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}