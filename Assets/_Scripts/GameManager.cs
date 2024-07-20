using System;
using UnityEngine;
/// <summary>
/// Basic GameManager to handle current state of the Game.
/// Contains Events to use for doing stuff when the GameState changes.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnBeforeGameStateChanged;
    public static event Action<GameState> OnAfterGameStateChanged;
    [SerializeField]
    public GameState State { get; private set; }

    //For test purposes:
    private void Start()
    {
        //Just for testing
        ChangeGameState(GameState.Starting);
    }


    public void ChangeGameState(GameState gameState)
    {

        if (State == gameState)
        {
            Debug.LogError("Duplicate GameState call, Game State is already " + gameState);
            return;
        }
        if (DebugManager.Instance.IsDebugModeActive(DebugManager.DebugModes.GameFlow))
        {
            Debug.Log("GameState changed to " + gameState + " from " + State + " by " + Time.time + " seconds.");
        }
        OnBeforeGameStateChanged?.Invoke(gameState);
        State = gameState;

        switch (gameState)
        {
            case GameState.MainMenu:
                GameSpeedManager.Instance.SetGameSpeed(GameSpeed.NORMAL);
                AudioSystem.Instance.PlayMenuMusic();
                break;
            case GameState.Starting:
                GameSpeedManager.Instance.SetGameSpeed(GameSpeed.NORMAL);
                AudioSystem.Instance.PlayMenuMusic();

                break;
            case GameState.BuildMode:
                GameSpeedManager.Instance.SetGameSpeed(GameSpeed.NORMAL);
                AudioSystem.Instance.PlayBuildMusic();
                UpgradeUI.Instance.Unselect();
                UIChangeManager.Instance.ShowBuildModeUI();
                UIChangeManager.Instance.HideUpgrades();
                RoundManager.Instance.NextRound();
                CardManager.Instance.ClearHand();
                CardManager.Instance.DrawRandomCards();
                Constants.Instance.DecreaseDamageToEnemiesMultiplier();
                break;
            case GameState.PlayMode:
                GameSpeedManager.Instance.SetGameSpeed(GameSpeed.NORMAL);
                AudioSystem.Instance.PlayBackgroundMusic();
                UIChangeManager.Instance.RemoveBuildModeUI();
                RoundManager.Instance.BeginNextRound();
                CardManager.Instance.DrawNewCards();
                UIChangeManager.Instance.ShowUpgrades();
                GameSpeedManager.Instance.SetNormal();
                break;
            case GameState.GameOver:
                AudioSystem.Instance.PlayGameOverMusic();
                UIChangeManager.Instance.ShowGameOver();
                CardManager.Instance.ClearHand();
                break;
            default:
                break;
        }

        OnAfterGameStateChanged?.Invoke(gameState);
    }

    public void ChangeGameStateToPlayMode()
    {
        ChangeGameState(GameState.PlayMode);
    }

}





[Serializable]
public enum GameState
{
    MainMenu = 0,
    Starting = 1,
    BuildMode = 2,
    PlayMode = 3,
    GameOver = 4,
}