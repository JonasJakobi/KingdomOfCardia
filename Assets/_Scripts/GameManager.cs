using System;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
    public static event Action<GameState> OnBeforeGameStateChanged;
    public static event Action<GameState> OnAfterGameStateChanged;
    public GameState State { get; private set; }

    private void Start()
    {
        ChangeGameState(GameState.MainMenu);

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
}