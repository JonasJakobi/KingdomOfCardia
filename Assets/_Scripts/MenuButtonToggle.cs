using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonScript : MonoBehaviour
{
    public GameObject optionsWindow;

    void Update()
    {
        checkEscPress();
    }

    // Method to toggle the options window
    public void ToggleOptionsWindow()
    {
        if (optionsWindow != null)
        {
            bool isActive = optionsWindow.activeSelf;
            if (isActive)
            {
                AudioSystem.Instance.PlayMenuClickSound();
                optionsWindow.SetActive(false);
                GameSpeedManager.Instance.SetGameSpeed(GameSpeed.NORMAL);

            }

            else
            {
                AudioSystem.Instance.PlayMenuClickSound();
                optionsWindow.SetActive(true);
                GameSpeedManager.Instance.SetGameSpeed(GameSpeed.PAUSE);
            }

        }
    }

    private void checkEscPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionsWindow();
        }
    }
}