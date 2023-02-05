using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DNA
{
    public class InputWrapper : MonoBehaviour
    {
        public enum UseAxis2D { x, y }

        [SerializeField] InputActionReference directionInput = null;
        [SerializeField] InputActionReference jumpButton = null;
        [SerializeField] InputActionReference escapeButton = null;

        public Vector2 Direction { get; private set; }
        public bool JumpActive { get; private set; }

        private void Awake()
        {
            directionInput.action.Enable();
            jumpButton.action.Enable();
            escapeButton.action.Enable();
        }

        private void OnDestroy()
        {
            directionInput.action.Disable();
            jumpButton.action.Disable();
            escapeButton.action.Disable();
        }

        void Update()
        {
            if (directionInput != null)
            {
                Direction = directionInput.ToInputAction().ReadValue<Vector2>();
            }

            if (jumpButton != null)
            {
                JumpActive = jumpButton.ToInputAction().ReadValue<float>() > 0f;
            }

            if(escapeButton != null)
            {
                if(escapeButton.ToInputAction().ReadValue<float>() > 0f)
                {
                    Application.Quit();
                }
            }
        }
    }
}