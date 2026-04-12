using System;
using System.Threading.Tasks;
using PlayMeow.Auth;
using PlayMeowInterview.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PlayMeow.UI
{
    public class LoginView : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TMP_InputField confirmPasswordInput;

        [Header("Buttons")]
        [FormerlySerializedAs("authButton")]
        [SerializeField] private Button authButton;
        [SerializeField] private TMP_Text authButtonText;
        [SerializeField] private Button googleLoginButton;
        [SerializeField] private Button forgotPasswordButton;
        [SerializeField] private Button switchAuthModeButton;
        [SerializeField] private TMP_Text switchAuthModeButtonText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button privacyPolicyButton;
        [SerializeField] private Button termsOfServiceButton;

        [Header("URLs")]
        [SerializeField] private string termsOfServiceUrl;
        [SerializeField] private string privacyPolicyUrl;

        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI inputHintText;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private UIShake errorTextShake;

        private AuthMode currentAuthMode = AuthMode.Login;

        private void Awake()
        {
            authButton.onClick.AddListener(OnAuthClicked);
            googleLoginButton.onClick.AddListener(OnGoogleLoginClicked);
            forgotPasswordButton.onClick.AddListener(OnForgotPasswordClicked);
            switchAuthModeButton.onClick.AddListener(SwitchAuthMode);
            closeButton.onClick.AddListener(OnCloseClicked);
            privacyPolicyButton.onClick.AddListener(OpenPrivacyPolicy);
            termsOfServiceButton.onClick.AddListener(OpenTermsOfService);

            emailInput.onValueChanged.AddListener(OnInputValueChanged);
            passwordInput.onValueChanged.AddListener(OnInputValueChanged);
            confirmPasswordInput.onValueChanged.AddListener(OnInputValueChanged);

            HideError();
            UpdateEmptyInputsText();
            SetAuthMode(currentAuthMode);
        }

        private void OnDestroy()
        {
            authButton.onClick.RemoveListener(OnAuthClicked);
            googleLoginButton.onClick.RemoveListener(OnGoogleLoginClicked);
            forgotPasswordButton.onClick.RemoveListener(OnForgotPasswordClicked);
            switchAuthModeButton.onClick.RemoveListener(SwitchAuthMode);
            closeButton.onClick.RemoveListener(OnCloseClicked);
            privacyPolicyButton.onClick.RemoveListener(OpenPrivacyPolicy);
            termsOfServiceButton.onClick.RemoveListener(OpenTermsOfService);

            emailInput.onValueChanged.RemoveListener(OnInputValueChanged);
            passwordInput.onValueChanged.RemoveListener(OnInputValueChanged);
            confirmPasswordInput.onValueChanged.RemoveListener(OnInputValueChanged);
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

            bool missingRequired = string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text);
            bool missingConfirm = currentAuthMode == AuthMode.SignUp && string.IsNullOrEmpty(confirmPasswordInput.text);
            inputHintText.enabled = missingRequired || missingConfirm;
        }

        private void OnAuthClicked()
        {
            switch (currentAuthMode)
            {
                case AuthMode.Login:
                    ExecuteLogin(() => AuthService.Instance.LoginAsync(emailInput.text, passwordInput.text));
                    break;
                case AuthMode.SignUp:
                    if (string.IsNullOrEmpty(confirmPasswordInput.text))
                    {
                        ShowError("請確認密碼");
                    }
                    else if (passwordInput.text != confirmPasswordInput.text)
                    {
                        ShowError("密碼不一致");
                    }
                    else
                    {
                        ExecuteLogin(() => AuthService.Instance.SignupAsync(emailInput.text, passwordInput.text));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnGoogleLoginClicked()
        {
            ExecuteLogin(() => AuthService.Instance.GoogleLoginAsync());
        }

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

        private void SwitchAuthMode()
        {
            var mode = currentAuthMode == AuthMode.Login ? AuthMode.SignUp : AuthMode.Login;
            SetAuthMode(mode);
        }

        private void SetAuthMode(AuthMode mode)
        {
            currentAuthMode = mode;

            confirmPasswordInput.gameObject.SetActive(mode == AuthMode.SignUp);

            switchAuthModeButtonText.text = mode switch
            {
                AuthMode.Login => "註冊新帳號",
                AuthMode.SignUp => "登入",
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };

            authButtonText.text = mode switch
            {
                AuthMode.Login => "登入",
                AuthMode.SignUp => "註冊新帳號",
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }

        private void OpenTermsOfService()
        {
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
            errorTextShake.Shake();
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
            authButton.interactable = interactable;
            googleLoginButton.interactable = interactable;
            emailInput.interactable = interactable;
            passwordInput.interactable = interactable;
            if (currentAuthMode == AuthMode.SignUp)
            {
                confirmPasswordInput.interactable = interactable;
            }
        }

        private void OnLoginSuccess(string token)
        {
            // TODO: store token and navigate to main scene
        }
    }
}