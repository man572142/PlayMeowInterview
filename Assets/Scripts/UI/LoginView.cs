using System;
using PlayMeow.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayMeow.UI
{
    public class LoginView : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;

        [Header("Buttons")]
        [SerializeField] private Button loginButton;
        [SerializeField] private Button googleLoginButton;
        [SerializeField] private Button forgotPasswordButton;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button closeButton;

        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI inputHintText;
        [SerializeField] private TextMeshProUGUI errorText;

        private void Awake()
        {
            loginButton.onClick.AddListener(OnLoginClicked);
            googleLoginButton.onClick.AddListener(OnGoogleLoginClicked);
            forgotPasswordButton.onClick.AddListener(OnForgotPasswordClicked);
            registerButton.onClick.AddListener(OnRegisterClicked);
            closeButton.onClick.AddListener(OnCloseClicked);

            emailInput.onValueChanged.AddListener(OnInputValueChanged);
            passwordInput.onValueChanged.AddListener(OnInputValueChanged);

            HideError();
            UpdateEmptyInputsText();
        }

        private void OnDestroy()
        {
            loginButton.onClick.RemoveListener(OnLoginClicked);
            googleLoginButton.onClick.RemoveListener(OnGoogleLoginClicked);
            forgotPasswordButton.onClick.RemoveListener(OnForgotPasswordClicked);
            registerButton.onClick.RemoveListener(OnRegisterClicked);
            closeButton.onClick.RemoveListener(OnCloseClicked);

            emailInput.onValueChanged.RemoveListener(OnInputValueChanged);
            passwordInput.onValueChanged.RemoveListener(OnInputValueChanged);
        }

        private void OnInputValueChanged(string _) => UpdateEmptyInputsText();

        private void UpdateEmptyInputsText()
        {
            if (inputHintText == null) return;
            
            inputHintText.enabled = string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text);
        }

        private async void OnLoginClicked()
        {
            HideError();
            SetInteractable(false);

            LoginResult result = await AuthService.Instance.LoginAsync(
                emailInput.text,
                passwordInput.text
            );

            SetInteractable(true);

            if (result.Success)
                OnLoginSuccess(result.Token);
            else
                ShowError(result.ErrorMessage);
        }

        private async void OnGoogleLoginClicked()
        {
            HideError();
            SetInteractable(false);

            LoginResult result = await AuthService.Instance.GoogleLoginAsync();

            SetInteractable(true);

            if (result.Success)
                OnLoginSuccess(result.Token);
            else
                ShowError(result.ErrorMessage);
        }

        private void OnForgotPasswordClicked()
        {
            Debug.Log("[LoginView] Forgot password tapped.");
            // TODO: navigate to forgot-password screen
        }

        private void OnRegisterClicked()
        {
            Debug.Log("[LoginView] Register tapped.");
            // TODO: navigate to registration screen
        }

        private void OnCloseClicked()
        {
            Debug.Log("[LoginView] Close tapped.");
            gameObject.SetActive(false);
        }

        private void ShowError(string msg)
        {
            if (errorText == null) return;
            errorText.text = msg;
            errorText.enabled = true;
        }

        private void HideError()
        {
            if (errorText == null) return;
            errorText.enabled = false;
        }

        private void SetInteractable(bool interactable)
        {
            loginButton.interactable = interactable;
            googleLoginButton.interactable = interactable;
            emailInput.interactable = interactable;
            passwordInput.interactable = interactable;
        }

        private void OnLoginSuccess(string token)
        {
            Debug.Log($"[LoginView] Login successful. Token: {token}");
            // TODO: store token and navigate to main scene
        }
    }
}
