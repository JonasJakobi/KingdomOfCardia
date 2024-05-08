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

    private void Start()
    {

    }


    public void ChangeGameState(GameState gameState)
    {
        if (State == gameState)
        {
            Debug.LogError("Duplicate GameState call, Game State is already " + gameState);
            return;
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
                break;
            case GameState.PlayMode:
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