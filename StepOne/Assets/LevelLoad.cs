using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoad : MonoBehaviour
{
    public GameObject LoadingBar;

    public GameObject Play;

    public GameObject Quit;

    public Slider PercentSlide;

    public void LoadLevel(int SceneIndex)
    {
        StartCoroutine(LoadAsynchronously(SceneIndex));
    }

    IEnumerator LoadAsynchronously(int SceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneIndex);

        SetLoading();

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            PercentSlide.value = progress;
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetLoading()
    {
        LoadingBar.SetActive(true);
        Play.SetActive(false);
        Quit.SetActive(false);
    }
}
