using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class GameSpeedManager : Singleton<GameSpeedManager>
{
    public GameSpeed gameSpeed;
    public float fastForwardTime = 3f;

    public Color color;
    public UnityEngine.UI.Image fastForwardImage;
    public UnityEngine.UI.Image pauseImage;
    public UnityEngine.UI.Image regularImage;
    public void SetGameSpeed(GameSpeed speed)
    {
        gameSpeed = speed;
        if (gameSpeed.Equals(GameSpeed.FAST))
        {
            Time.timeScale = fastForwardTime;
        }
        else if (gameSpeed.Equals(GameSpeed.PAUSE))
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        UpdateButtonImages();
    }

    public void SetFast()
    {
        SetGameSpeed(GameSpeed.FAST);
    }
    public void SetPause()
    {
        SetGameSpeed(GameSpeed.PAUSE);
    }
    public void SetNormal()
    {
        SetGameSpeed(GameSpeed.NORMAL);
    }
    private void UpdateButtonImages()
    {
        fastForwardImage.color = (gameSpeed.Equals(GameSpeed.FAST)) ? color : Color.white;
        pauseImage.color = (gameSpeed.Equals(GameSpeed.PAUSE)) ? color : Color.white;
        regularImage.color = (gameSpeed.Equals(GameSpeed.NORMAL)) ? color : Color.white;
    }


}
[System.Serializable]
public enum GameSpeed
{
    NORMAL, FAST, PAUSE
}
