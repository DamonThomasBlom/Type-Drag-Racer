using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject LoginCanvas;
    public GameObject MainMenuCanvas;

    [Header("Pages")]
    public GameObject LoadingOverlay;
    public UISlideIn LoginPanel;
    public UISlideIn SignupPanel;

    [Header("Login")]
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public Toggle RememberMeToggle;
    public Button LoginBtn;
    public Button SignupBtn;

    [Header("Registration")]
    public TMP_InputField RegUsernameInput;
    public TMP_InputField RegEmailInput;
    public TMP_InputField RegPasswordInput;
    public Button RegisterBtn;
    public GameObject UserDetailsPanel;
    public GameObject RegistrationSentPanel;
    public Button RegBackBtn;
    public Button BackToLoginBtn;

    private void Awake()
    {
        LoginBtn.onClick.AddListener(LoginUser);

        SignupBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(SignupPanel, !SignupPanel.On)));
        RegBackBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(LoginPanel, !LoginPanel.On)));
        BackToLoginBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(LoginPanel, !LoginPanel.On)));

        RegisterBtn.onClick.AddListener(RegisterUser);

        // Load login panel
        TogglePanel(LoginPanel, true);
        TogglePanel(SignupPanel, false);

        // Load saved credentials if any are saved
        LoadCredentials();

        // If we come back to this scene and we are already logged in go to next page
        if (!string.IsNullOrEmpty(Player.Instance.Token))
        {
            ShowLoadingPanel(false);
            MainMenuCanvas.SetActive(true);
            LoginCanvas.SetActive(false);
        }
    }

    void RegisterUser()
    {
        if (string.IsNullOrEmpty(RegUsernameInput.text))
        {
            ToastManager.Instance.ShowToast("Username can not be blank.");
            return;
        }

        ShowLoadingPanel(true);
        DatabaseManager.Instance.RegisterUser(RegUsernameInput.text, RegEmailInput.text, RegPasswordInput.text, (callback) =>
        {
            Debug.Log($"Register Success: {callback.IsSuccess}  Message: {callback.Message}");
            ShowLoadingPanel(false);

            if (callback.IsSuccess)
            {
                UserDetailsPanel.SetActive(false);
                RegistrationSentPanel.SetActive(true);
            }
            else
            {
                ToastManager.Instance.ShowToast(callback.Message);
            }
        });
    }

    void LoginUser()
    {
        if (string.IsNullOrEmpty(EmailInput.text))
        {
            ToastManager.Instance.ShowToast("Email can not be blank.");
            return;
        }

        if (string.IsNullOrEmpty(PasswordInput.text))
        {
            ToastManager.Instance.ShowToast("Password can not be blank.");
            return;
        }

        ShowLoadingPanel(true);
        DatabaseManager.Instance.Login(EmailInput.text, PasswordInput.text, (callback) =>
        {
            Debug.Log($"Login Success: {callback.IsSuccess}  Message: {callback.Message}");

            if (callback.IsSuccess)
            {
                SaveCredentials();
                DelayedActionUtility.Instance.PerformActionWithDelay(1f, () =>
                {
                    ShowLoadingPanel(false);
                    MainMenuCanvas.SetActive(true);
                    LoginCanvas.SetActive(false);
                });
            }
            else
            {
                ShowLoadingPanel(false);
                ToastManager.Instance.ShowToast(callback.Message);
            }
        });
    }

    void SaveCredentials()
    {
        if (RememberMeToggle.isOn)
        {
            PlayerPrefs.SetString("SavedEmail", EmailInput.text);
            PlayerPrefs.SetString("SavedPassword", PasswordInput.text);
            PlayerPrefs.SetInt("RememberMe", 1);
        }
        else
        {
            PlayerPrefs.DeleteKey("SavedEmail");
            PlayerPrefs.DeleteKey("SavedPassword");
            PlayerPrefs.SetInt("RememberMe", 0);
        }
        PlayerPrefs.Save();
    }

    void LoadCredentials()
    {
        if (PlayerPrefs.GetInt("RememberMe", 0) == 1)
        {
            EmailInput.text = PlayerPrefs.GetString("SavedEmail", "");
            PasswordInput.text = PlayerPrefs.GetString("SavedPassword", "");
            RememberMeToggle.isOn = true;
        }
    }

    void CloseAllPanels(Action callback)
    {
        Debug.Log("Close all panels");
        if (callback == null)
        {
            LoginPanel.SlideOut();
            SignupPanel.SlideOut();

            return;
        }

        LoginPanel.SlideOut(callback);
        SignupPanel.SlideOut(callback);
    }

    void TogglePanel(UISlideIn panel, bool enabled)
    {
        Debug.Log($"{panel.name} panel: {enabled}");

        if (enabled)
        {
            panel.gameObject.SetActive(true);
            panel.SlideIn();
        }
        else
        {
            panel.SlideOut(() => panel.gameObject.SetActive(false));
        }

        // If we are going back to login panel clean up registration UI
        if (panel == LoginPanel)
            CleanUpRegistration();
    }

    void ShowLoadingPanel(bool enabled)
    {
        LoadingOverlay.gameObject.SetActive(enabled);
    }

    private void CleanUpRegistration()
    {
        RegUsernameInput.text = "";
        RegEmailInput.text = "";
        RegPasswordInput.text = "";

        UserDetailsPanel.SetActive(true);
        RegistrationSentPanel.SetActive(false);
    }
}
