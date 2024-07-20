using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuFunctions : MonoBehaviour
{
    public GameObject mainMenu;

    public GameObject settings;

    public Toggle enabledTutorial;
    // Start is called before the first frame update
    void Start()
    {
        enabledTutorial.isOn = ConsistentSettings.tutorialEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMenuEscPress();
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseSettings()
    {
        settings.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ToggleDisableTutorialPermanently()
    {
        ConsistentSettings.tutorialEnabled = enabledTutorial.isOn;
    }

    private void CheckMenuEscPress()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && settings.activeSelf)
        {
            CloseSettings();
        }
    }
}
