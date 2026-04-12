using System;
using System.Threading.Tasks;
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
        [SerializeField] private SpriteToggleButton passwordVisibilityButton;
        [SerializeField] private Button privacyPolicyButton;
        [SerializeField] private Button termsOfServiceButton;
        
        [Header("URLs")]
        [SerializeField] private string termsOfServiceUrl;
        [SerializeField] private string privacyPolicyUrl;

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
            privacyPolicyButton.onClick.AddListener(OpenPrivacyPolicy);
            termsOfServiceButton.onClick.AddListener(OpenTermsOfService);

            emailInput.onValueChanged.AddListener(OnInputValueChanged);
            passwordInput.onValueChanged.AddListener(OnInputValueChanged);
            passwordVisibilityButton.OnValueChanged += SetPasswordVisible;

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
            privacyPolicyButton.onClick.RemoveListener(OpenPrivacyPolicy);
            termsOfServiceButton.onClick.RemoveListener(OpenTermsOfService);

            emailInput.onValueChanged.RemoveListener(OnInputValueChanged);
            passwordInput.onValueChanged.RemoveListener(OnInputValueChanged);
            passwordVisibilityButton.OnValueChanged -= SetPasswordVisible;
        }

        private void OnInputValueChanged(string _)
        {
            UpdateEmptyInputsText();
        }

        private void UpdateEmptyInputsText()
        {
            if (inputHintText == null)
            {
                return;
            }

            inputHintText.enabled = string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text);
        }

        private void OnLoginClicked() =>
            ExecuteLogin(() => AuthService.Instance.LoginAsync(emailInput.text, passwordInput.text));

        private void OnGoogleLoginClicked() =>
            ExecuteLogin(() => AuthService.Instance.GoogleLoginAsync());

        private async void ExecuteLogin(Func<Task<LoginResult>> loginFunc)
        {
            HideError();
            SetInteractable(false);

            var result = await loginFunc();

            if (this == null)
            {
                return;
            }

            SetInteractable(true);

            if (result.Success)
            {
                OnLoginSuccess(result.Token);
            }
            else
            {
                ShowError(result.ErrorMessage);
            }
        }

        private void OnForgotPasswordClicked()
        {
            // TODO: navigate to forgot-password screen
        }

        private void OnRegisterClicked()
        {
            // TODO: navigate to registration screen
        }

        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }
        
        private void OpenTermsOfService()
        {
            Debug.Log("[LoginView] Terms of service tapped.");
            Application.OpenURL(termsOfServiceUrl);
        }

        private void OpenPrivacyPolicy()
        {
            Application.OpenURL(privacyPolicyUrl);
        }

        private void ShowError(string msg)
        {
            if (errorText == null)
            {
                return;
            }

            errorText.text = msg;
            errorText.enabled = true;
        }

        private void HideError()
        {
            if (errorText == null)
            {
                return;
            }

            errorText.enabled = false;
        }

        private void SetInteractable(bool interactable)
        {
            loginButton.interactable = interactable;
            googleLoginButton.interactable = interactable;
            emailInput.interactable = interactable;
            passwordInput.interactable = interactable;
        }

        public void SetPasswordVisible(bool visible)
        {
            passwordInput.inputType = visible
                ? TMP_InputField.InputType.Standard
                : TMP_InputField.InputType.Password;
            passwordInput.ForceLabelUpdate();
        }

        private void OnLoginSuccess(string token)
        {
            // TODO: store token and navigate to main scene
        }
    }
}