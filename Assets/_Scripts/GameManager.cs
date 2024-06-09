using System;
using UnityEngine;
/// <summary>
/// Basic GameManager to handle current state of the Game.
/// Contains Events to use for doing stuff when the GameState changes.
/// </summary>
public class GameManager : SingletonPersistent<GameManager>
{
    public static event Action<GameState> OnBeforeGameStateChanged;
    public static event Action<GameState> OnAfterGameStateChanged;
    [SerializeField]
    public GameState State { get; private set; }
    //For test purposes:
    public GameObject towerPlaceUI;
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
                break;
            case GameState.Starting:
                break;
            case GameState.BuildMode:
                towerPlaceUI.SetActive(true);
                RoundManager.Instance.NextRound();
                CardManager.Instance.ClearHand();
                CardManager.Instance.DrawRandomCards();
                break;
            case GameState.PlayMode:
                towerPlaceUI.SetActive(false);
                RoundManager.Instance.BeginNextRound();
                CardManager.Instance.DrawNewCards(3);
                UIChangeManager.Instance.removeAllWaveAlerts();
                break;
            default:
                break;
        }

        OnAfterGameStateChanged?.Invoke(gameState);
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