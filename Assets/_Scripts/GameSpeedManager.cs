using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class GameSpeedManager : Singleton<GameSpeedManager>
{
    public bool pauseMode = false;
    public bool fastForwardMode = false;
    public float fastForwardTime = 3f;

    public Color color;
    public UnityEngine.UI.Image fastForwardImage;
    public UnityEngine.UI.Image pauseImage;
    public void SetFastForward(bool ff)
    {
        if (pauseMode)
        {
            SetPauseMode(false);
        }

        fastForwardMode = ff;
        if (!fastForwardMode)
        {
            Time.timeScale = 1;
            fastForwardImage.color = Color.white;

        }
        else
        {
            Time.timeScale = fastForwardTime;
            fastForwardImage.color = color;

        }
    }

    public void SetPauseMode(bool pause)
    {
        if (fastForwardMode)
        {
            SetFastForward(false);
        }

        pauseMode = pause;
        if (!pauseMode)
        {
            Time.timeScale = 1;
            pauseImage.color = Color.white;

        }
        else
        {
            Time.timeScale = 0;
            pauseImage.color = color;

        }
    }
    public void ToggleFastFordwardMode()
    {
        SetFastForward(!fastForwardMode);
    }
    public void TogglePauseMode()
    {
        SetPauseMode(!pauseMode);
    }


}
