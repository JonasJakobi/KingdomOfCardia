using UnityEngine;

public class DifficultySelector : MonoBehaviour
{
    public GameDifficulty easyDifficulty;
    public GameDifficulty mediumDifficulty;
    public GameDifficulty hardDifficulty;

    public GameDifficulty sandboxDifficulty;
    public GameDifficulty fastEnemies;
    private void SetDifficulty(GameDifficulty difficulty)
    {
        Constants.selectedDifficulty = difficulty;
    }

    public void SetEasyDifficulty()
    {
        SetDifficulty(easyDifficulty);
    }
    public void SetMediumDifficulty()
    {
        SetDifficulty(mediumDifficulty);
    }
    public void SetHardDifficulty()
    {
        SetDifficulty(hardDifficulty);
    }
    public void SetSandboxDifficulty()
    {
        SetDifficulty(sandboxDifficulty);
    }
    public void SetFastEnemies()
    {
        SetDifficulty(fastEnemies);
    }

}