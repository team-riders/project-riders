using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using RidersRuntime.Data;
using UnityEngine;
using System.Threading.Tasks;

namespace RidersRuntime
{
    public static class SceneLoader
    {
        private static bool isLoading = false;
        public static Action loadingScreenStartUnloading;

        /// <summary>
        /// Stop loading the screen once we hit this percentage
        /// </summary>
        private static float loadingScreenPercentageLock = 0.9f;
        private static GameScene targetGameScene;
        private static GameScene targetLoadingScene;
        public static AsyncOperation currentlyTryingToLoadScene;

        public static bool SceneIsready
        {
            get
            {
                if (currentlyTryingToLoadScene == null)
                {
                    return false;
                }
                return currentlyTryingToLoadScene.progress >= loadingScreenPercentageLock;
            }
        }
        private static Action onLoadActualscene;

        /// <summary>
        /// This will present the loading screen while having the the previous scene nearly ready to be loaded
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="targetLoadingSceneIndex">Use if you want to use a special loading scene</param>
        /// <returns></returns>
        public static async Task PrepareScene(GameScene scene, Action onLoad = null, GameScene loadingScene = GameScene.Loading)
        {
            if (isLoading) return;
            isLoading = true;

            // Disable input during loading
            if (EventSystem.current != null)
            {
                EventSystem.current.enabled = false;
            }

            onLoadActualscene = onLoad;

            targetGameScene = scene;
            targetLoadingScene = loadingScene;

            // Sets up the loading screen
            await DoLoadingScreenTask((int)targetLoadingScene);
        }

        public static async Task PrepareScene(int scene, Action onLoad = null, GameScene loadingScene = GameScene.Loading)
        {
            if (isLoading) return;
            isLoading = true;

            // Disable input during loading
            if (EventSystem.current != null)
            {
                EventSystem.current.enabled = false;
            }

            onLoadActualscene = onLoad;

            targetGameScene = (GameScene)scene;
            targetLoadingScene = loadingScene;

            // Sets up the loading screen
            await DoLoadingScreenTask(scene);
        }

        /// <summary>
        /// This will belong in the loading scene that is used. It will be called when the developer wants to do it
        /// It can be either (when the new scene is fully loaded) or (when the player presses a button). i.e. (automatic or manual)
        /// </summary>
        /// <returns></returns>
        public static async Task ActivateNewScene()
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.enabled = false;
            }

            currentlyTryingToLoadScene.allowSceneActivation = true;

            while (!currentlyTryingToLoadScene.isDone)
            {
                await Task.Yield();
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)targetGameScene));
        }

        public static async void LoadingScreenEntry()
        {
            await InternalUnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            DoSceneLoadingTask((int)targetGameScene);

            // SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)GameScene.Loading));
        }

        public static async void LoadingScreenExit()
        {
            await InternalUnloadSceneAsync(SceneManager.GetSceneByBuildIndex((int)targetLoadingScene).buildIndex);
            onLoadActualscene?.Invoke();
            onLoadActualscene = null;

            isLoading = false;
            currentlyTryingToLoadScene = null;
        }

        public static float GetCurrentLoadingProgress()
        {
            if (currentlyTryingToLoadScene == null)
            {
                return 0f;
            }
            return currentlyTryingToLoadScene.progress;
        }

        private static async Task DoLoadingScreenTask(int loadingSceneIndex)
        {
            await InternalLoadSceneAsync(loadingSceneIndex);

            // There may be an event system in the loading scene
            if (EventSystem.current != null)
            {
                EventSystem.current.enabled = true;
            }
        }

        private static void DoSceneLoadingTask(int sceneIndex)
        {
            isLoading = true;
            currentlyTryingToLoadScene = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            currentlyTryingToLoadScene.allowSceneActivation = false;
        }

        private static async Task InternalLoadSceneAsync(int sceneIndex)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }

        private static async Task InternalUnloadSceneAsync(int sceneIndex)
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneIndex);

            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }
    }
}