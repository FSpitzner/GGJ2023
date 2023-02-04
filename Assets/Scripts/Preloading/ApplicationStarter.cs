using DG.Tweening;
using UnityEngine;

namespace DNA
{
    /// <summary>
    /// Initializes relevant data in Preload Scene.
    /// </summary>
    public class ApplicationStarter : MonoBehaviour
    {
        void Awake()
        {
            // Hide mouse cursor:
            Cursor.visible = false;

            // Initialize DOTween:
            DOTween.Init();
        }
    }
}