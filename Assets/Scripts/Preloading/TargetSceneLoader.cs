using UnityEngine;
using UnityEngine.SceneManagement;

namespace DNA
{
    /// <summary>
    /// Returns to target scene when running game in Editor Mode, after Preload Scene has been initialized.
    /// </summary>
    public class TargetSceneLoader : MonoBehaviour
    {

        private void Awake()
        {
#if UNITY_EDITOR
            int sceneIndex = LoadingSceneIntegration.otherScene > 0 ? LoadingSceneIntegration.otherScene : 1;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
#endif
#if UNITY_STANDALONE
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
#endif
        }
    }
}