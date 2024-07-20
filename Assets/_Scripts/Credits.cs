using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] private CanvasGroup skipText;
    private bool skipShown = false;

    // Start is called before the first frame update
    void Start()
    {
        AudioSystem.Instance.PlayGameOverMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (!skipShown) CheckAnyKeyPress();
        else if (skipShown) CheckCreditEscPress();

    }

    public void ShowSkip()
    {
        StartCoroutine(SkipTimer(2.5f));
    }

    private void CheckCreditEscPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && skipShown)
        {
            LoadMainMenu("MainMenu");
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !skipShown)
        {
            ShowSkip();
        }
    }

    private void CheckAnyKeyPress()
    {
        if (Input.anyKeyDown) ShowSkip();
    }

    private void LoadMainMenu(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " cannot be loaded.");

        }
    }

    private IEnumerator SkipTimer(float showDuration)
    {
        float duration = 0.5f;
        skipShown = true;
        float time = 0f;
        while (time < duration)
        {
            skipText.alpha = time * duration * 2;
            time += Time.deltaTime;
            yield return null;
        }
        skipText.alpha = 1;
        yield return new WaitForSeconds(showDuration);
        time = duration;
        while (time > 0f)
        {
            skipText.alpha = time * duration * 2;
            time -= Time.deltaTime;
            yield return null;
        }
        skipText.alpha = 0;
        skipShown = false;
    }


}
