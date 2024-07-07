using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonScript : MonoBehaviour
{
    public GameObject optionsWindow;
    public GameObject quitWindow;
    public GameObject menuBackground;

    void Update()
    {
        checkEscPress();
    }

    // Method to toggle the options window
    public void ToggleOptionsWindow(bool toQuit)
    {

        if (optionsWindow.activeSelf && !toQuit)
        {
            AudioSystem.Instance.PlayMenuClickSound();
            menuBackground.SetActive(false);
            optionsWindow.SetActive(false);
            quitWindow.SetActive(false);
            GameSpeedManager.Instance.SetGameSpeed(GameSpeed.NORMAL);

        }

        else if (optionsWindow.activeSelf && toQuit)
        {
            AudioSystem.Instance.PlayMenuClickSound();
            menuBackground.SetActive(true);
            optionsWindow.SetActive(false);
            quitWindow.SetActive(true);
        }

        else
        {
            AudioSystem.Instance.PlayMenuClickSound();
            menuBackground.SetActive(true);
            optionsWindow.SetActive(true);
            quitWindow.SetActive(false);
            GameSpeedManager.Instance.SetGameSpeed(GameSpeed.PAUSE);
        }
    }

    private void checkEscPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionsWindow(false);
        }
    }
}