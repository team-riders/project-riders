using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine;

public static class SceneLoader
{
    private static bool isLoading = false;
    private static Action _loadingScreenLoaded;
    private static Action _loadingScreenUnloaded;
    public static Action loadingScreenStartUnloading;

    public static void LoadScene(Scene scene)
    {
        if (isLoading) return;
        isLoading = true;

        // Disable input during loading
        if (EventSystem.current != null)
        {
            EventSystem.current.enabled = false;
        }

        // Refresh scene event listeners
        SceneManager.sceneUnloaded -= SceneUnloaded;
        SceneManager.sceneLoaded -= SceneLoaded;

        SceneManager.sceneUnloaded += SceneUnloaded;
        SceneManager.sceneLoaded += SceneLoaded;

        _loadingScreenUnloaded = () =>
        {
            isLoading = false;
            SceneManager.UnloadSceneAsync((int)Scene.Loading);
        };

        _loadingScreenLoaded = () =>
        {
            Core.shared.StartCoroutine(InternalSceneLoad(scene));
            _loadingScreenLoaded = null;
        };

        var loadingScene = SceneManager.GetSceneByBuildIndex((int)Scene.Loading);
        if (loadingScene.isLoaded)
        {
            _loadingScreenLoaded?.Invoke();
            SceneManager.LoadScene("", LoadSceneMode.Additive);
            return;
        }

        SceneManager.LoadSceneAsync((int)Scene.Loading, LoadSceneMode.Additive);
    }

    private static IEnumerator InternalSceneLoad(Scene scene)
    {
        // Begin loading target scene
        var operation = SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        // Unload current scene (not loading screen)
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        // Set new scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)scene));

        // Fade out / clean up loading screen
        loadingScreenStartUnloading?.Invoke();

        yield return null;
    }

    private static void SceneUnloaded(UnityEngine.SceneManagement.Scene scene) { }

    private static void SceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == (int)Scene.Loading)
        {
            _loadingScreenLoaded?.Invoke();
        }
        else
        {
            _loadingScreenUnloaded?.Invoke();
        }
    }
}
