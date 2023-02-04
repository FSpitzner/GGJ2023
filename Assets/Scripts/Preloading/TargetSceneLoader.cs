using UnityEngine;
using UnityEngine.SceneManagement;

namespace DNA
{
    /// <summary>
    /// Returns to target scene when running game in Editor Mode, after Preload Scene has been initialized.
    /// </summary>
    public class TargetSceneLoader : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Awake()
        {
            int sceneIndex = LoadingSceneIntegration.otherScene > 0 ? LoadingSceneIntegration.otherScene : 1;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }
#endif
    }
}