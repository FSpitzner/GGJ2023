using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DNA
{
    public class ApplicationQuitter : MonoBehaviour
    {
        [SerializeField] InputActionReference button1Ref;
        [SerializeField] InputActionReference button2Ref;

        private void Awake()
        {
            button1Ref.action.Enable();
            button2Ref.action.Enable();
        }

        private void OnDestroy()
        {
            button1Ref.action.Dispose();
            button2Ref.action.Dispose();
        }

        private void Update()
        {
            if (button1Ref)
            {
                if (button1Ref.ToInputAction().ReadValue<float>() > 0)
                    Application.Quit();
            }

            if (button2Ref)
            {
                if (button2Ref.ToInputAction().ReadValue<float>() > 0)
                    Application.Quit();
            }
        }
    }
}