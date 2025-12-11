using _Project.Scripts.EventStructs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPopup : TitlePopup
{
    public override int PanelType => (int)TitlePanelType.SignUp;
    
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;
    
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button cancelButton;
    
    private void OnEnable()
    {
        emailInput.SetTextWithoutNotify("");
        passwordInput.SetTextWithoutNotify("");
        confirmPasswordInput.SetTextWithoutNotify("");
    }

    private void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    private void OnDestroy()
    {
        signUpButton.onClick.RemoveListener(OnSignUpButtonClick);
        cancelButton.onClick.RemoveListener(OnCancelButtonClick);
    }

    private void OnSignUpButtonClick()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;
        
        EventHub.Instance?.RaiseEvent(new FirebaseEvents.RequestCreateEmailEvent(email, password, confirmPassword));
    }

    private void OnCancelButtonClick()
    {
        titleUIController.ChangePanel(TitlePanelType.SignIn);
    }
}
