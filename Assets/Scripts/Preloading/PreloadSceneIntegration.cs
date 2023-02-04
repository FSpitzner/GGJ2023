using UnityEngine;
using UnityEngine.SceneManagement;

namespace DNA
{
    /// <summary>
    /// Makes sure to load Preload Scene first when starting game in Editor Mode, which guarantees all global manager objects to be loaded.
    /// </summary>
    public class LoadingSceneIntegration
    {

#if UNITY_EDITOR
        public static int otherScene = -2; // temporary cache for target scene ID

        // Run instantly when starting game in Editor Mode:
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitLoadingScene()
        {
            // Check if preload scene is already loaded:
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex == 0) return;

            // Load preload scene to initialize global manager objects:
            otherScene = sceneIndex;
            SceneManager.LoadScene(0);
        }
#endif
    }
}