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
        [SerializeField] private TextMeshProUGUI errorText;

        private void Awake()
        {
            loginButton.onClick.AddListener(OnLoginClicked);
            googleLoginButton.onClick.AddListener(OnGoogleLoginClicked);
            forgotPasswordButton.onClick.AddListener(OnForgotPasswordClicked);
            registerButton.onClick.AddListener(OnRegisterClicked);
            closeButton.onClick.AddListener(OnCloseClicked);

            HideError();
        }

        private void OnDestroy()
        {
            loginButton.onClick.RemoveListener(OnLoginClicked);
            googleLoginButton.onClick.RemoveListener(OnGoogleLoginClicked);
            forgotPasswordButton.onClick.RemoveListener(OnForgotPasswordClicked);
            registerButton.onClick.RemoveListener(OnRegisterClicked);
            closeButton.onClick.RemoveListener(OnCloseClicked);
        }

        public async void OnLoginClicked()
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

        public async void OnGoogleLoginClicked()
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

        public void OnForgotPasswordClicked()
        {
            Debug.Log("[LoginView] Forgot password tapped.");
            // TODO: navigate to forgot-password screen
        }

        public void OnRegisterClicked()
        {
            Debug.Log("[LoginView] Register tapped.");
            // TODO: navigate to registration screen
        }

        public void OnCloseClicked()
        {
            Debug.Log("[LoginView] Close tapped.");
            gameObject.SetActive(false);
        }

        public void ShowError(string msg)
        {
            if (errorText == null) return;
            errorText.text = msg;
            errorText.gameObject.SetActive(true);
        }

        public void HideError()
        {
            if (errorText == null) return;
            errorText.gameObject.SetActive(false);
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
