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
            AudioSystem.Instance.PlayMenuClickSound();
            bool isActive = optionsWindow.activeSelf;
            optionsWindow.SetActive(!isActive);
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