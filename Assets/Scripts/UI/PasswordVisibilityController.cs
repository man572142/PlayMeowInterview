
using System;
using TMPro;
using UnityEngine;

namespace PlayMeow.UI
{
    public class PasswordVisibilityController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private SpriteToggleButton visibilityToggle;

        void Awake()
        {
            visibilityToggle.OnValueChanged += SetPasswordVisible;
        }

        private void OnDestroy()
        {
            visibilityToggle.OnValueChanged -= SetPasswordVisible;
        }

        private void SetPasswordVisible(bool visible)
        {
            inputField.inputType = visible
                ? TMP_InputField.InputType.Standard
                : TMP_InputField.InputType.Password;
            inputField.ForceLabelUpdate();
        }
    }

}