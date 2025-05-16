using System.Collections;
using RidersRuntime.GameSystems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RidersRuntime
{
    public class LoadingScreenBehaviour : MonoBehaviour
    {
        Animator animator;

        public AnimationClip loadingScreenAnimation;
        public AnimationClip loadingScreenExitAnimation;
        public GameObject startButton;

        bool isLoaded = false;

        MultiDeviceControllerSystem mdcs;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Will generally be invoked anyways, but we can make a public method in case
            EnterLoadingScreen();
            mdcs = FindFirstObjectByType<MultiDeviceControllerSystem>();
            mdcs.CanJoin(false);
        }

        void EnterLoadingScreen()
        {
            // This will be called when the loading screen is activated
            // We can set up the loading screen here
            // Check if we actually have a loading reference
            animator = GetComponent<Animator>();
            animator.SetTrigger("LoadingScreen");
        }

        void Update()
        {
            float loadingProgress = SceneLoader.GetCurrentLoadingProgress();

            if (SceneLoader.SceneIsready && !isLoaded)
            {
                isLoaded = true;
                StartCoroutine(FakeLoadingProgress(3));
            }
        }

        IEnumerator FakeLoadingProgress(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            startButton.SetActive(true);
            // EventSystem.current.SetSelectedGameObject(startButton);
            // Just call it
            DoReady();
        }

        public void FinishLoadingScreenEntry()
        {
            SceneLoader.LoadingScreenEntry();
        }

        public async void DoReady()
        {
            await SceneLoader.ActivateNewScene();
            ExitLoadingScreen();
        }

        public void FinishLoadingScreenExit()
        {
            // This will be called when the loading screen is deactivated
            SceneLoader.LoadingScreenExit();
        }

        public void ExitLoadingScreen()
        {
            // This will be called when the loading screen is deactivated
            animator.SetTrigger("LoadingScreenExit");
        }
    }
}
