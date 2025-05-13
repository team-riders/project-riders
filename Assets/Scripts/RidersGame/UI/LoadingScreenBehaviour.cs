using UnityEditor.SearchService;
using UnityEngine;

namespace RidersRuntime
{
    public class LoadingScreenBehaviour : MonoBehaviour
    {
        Animator animator;

        public AnimationClip loadingScreenAnimation;
        public AnimationClip loadingScreenExitAnimation;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Will generally be invoked anyways, but we can make a public method in case
            EnterLoadingScreen();
        }

        void EnterLoadingScreen()
        {
            // This will be called when the loading screen is activated
            // We can set up the loading screen here
            // Check if we actually have a loading reference

            bool isLoading = SceneLoader.GetCurrentLoadingProgress() > 0.0f;
            animator = GetComponent<Animator>();
            animator.SetTrigger("LoadingScreen");
        }

        void Update()
        {
            float loadingProgress = SceneLoader.GetCurrentLoadingProgress();

            if (SceneLoader.SceneIsready)
            {
            }
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
