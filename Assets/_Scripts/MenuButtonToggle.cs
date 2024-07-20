using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonScript : MonoBehaviour
{
    public GameObject optionsWindow;
    public GameObject quitWindow;
    public GameObject menuBackground;
    public GameObject settingsWindow;

    void Update()
    {
        CheckEscPress();
    }

    // Method to toggle the options window
    public void ToggleOptionsWindow(int stateOfWindow)
    {

        if (optionsWindow.activeSelf && stateOfWindow == 0)
        {
            AudioSystem.Instance.PlayMenuClickSound();
            menuBackground.SetActive(false);
            optionsWindow.SetActive(false);
            quitWindow.SetActive(false);
            settingsWindow.SetActive(false);
            GameSpeedManager.Instance.SetGameSpeed(GameSpeed.NORMAL);

        }

        else if (!optionsWindow.activeSelf && stateOfWindow == 0)
        {
            AudioSystem.Instance.PlayMenuClickSound();
            menuBackground.SetActive(true);
            optionsWindow.SetActive(true);
            quitWindow.SetActive(false);
            settingsWindow.SetActive(false);
            GameSpeedManager.Instance.SetGameSpeed(GameSpeed.PAUSE);
        }

        else if (optionsWindow.activeSelf && stateOfWindow == 1)
        {
            AudioSystem.Instance.PlayMenuClickSound();
            menuBackground.SetActive(true);
            optionsWindow.SetActive(true);
            quitWindow.SetActive(false);
            settingsWindow.SetActive(false);
            GameSpeedManager.Instance.SetGameSpeed(GameSpeed.PAUSE);
        }

        else if (optionsWindow.activeSelf && stateOfWindow == 2)
        {
            AudioSystem.Instance.PlayMenuClickSound();
            menuBackground.SetActive(true);
            optionsWindow.SetActive(false);
            quitWindow.SetActive(false);
            settingsWindow.SetActive(true);
        }

        else if (optionsWindow.activeSelf && stateOfWindow == 3)
        {
            AudioSystem.Instance.PlayMenuClickSound();
            menuBackground.SetActive(true);
            optionsWindow.SetActive(false);
            quitWindow.SetActive(true);
            settingsWindow.SetActive(false);
        }
    }



    private void CheckEscPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionsWindow(0);
        }
    }
}