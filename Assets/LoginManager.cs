using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
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
        SignupBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(SignupPanel, !SignupPanel.On)));
        RegBackBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(LoginPanel, !LoginPanel.On)));
        BackToLoginBtn.onClick.AddListener(() => CloseAllPanels(() => TogglePanel(LoginPanel, !LoginPanel.On)));

        RegisterBtn.onClick.AddListener(RegisterUser);

        // Load login panel
        TogglePanel(LoginPanel, true);
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
