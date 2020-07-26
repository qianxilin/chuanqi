using System;
using UnityEngine;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class LoginManager : MonoBehaviour
{
    //Connection
    public GameObject ConnectBox;
    //Login
    public GameObject LoginBox;
    public GameObject UserName;
    public GameObject Password;
    //Register
    public GameObject RegisterBox;
    public GameObject RegisterUserName;
    public GameObject RegisterPassword;
    public GameObject ConfirmPassword;
    //Misc
    public GameObject MessageBox;

    private bool loginshown = false;

    // Start is called before the first frame update
    void Start()
    {
        LoginBox.SetActive(false);
        RegisterBox.SetActive(false);
        ConnectBox.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!loginshown)
        {
            if (Network.LoginConnected)
            ShowLoginBox();
            Network.LoginManager = gameObject;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (UserName.GetComponent<TMP_InputField>().isFocused)
                    Password.GetComponent<TMP_InputField>().Select();
                else if (Password.GetComponent<TMP_InputField>().isFocused)
                    UserName.GetComponent<TMP_InputField>().Select();
            }
        }
    }

    public void ShowLoginBox()
    {
        loginshown = true;
        ConnectBox.SetActive(false);
        LoginBox.SetActive(true);
        UserName.GetComponent<TMP_InputField>().ActivateInputField();
    }

    public void ShowMessageBox(string message)
    {
        MessageBox.GetComponentInChildren<TextMeshProUGUI>().text = message;
        MessageBox.SetActive(true);
    }

    public void LoginButton_OnClick()
    {
        string username = UserName.GetComponent<TMP_InputField>().text;
        string password = Password.GetComponent<TMP_InputField>().text;

        if (username == string.Empty || password == string.Empty) return;

        Network.Enqueue(new C.Login
        {
            AccountID = username,
            Password = password
        });

        UserName.GetComponent<TMP_InputField>().text = string.Empty;
        Password.GetComponent<TMP_InputField>().text = string.Empty;
    }

    public void RegisterButton_OnClick()
    {
        string username = RegisterUserName.GetComponent<TMP_InputField>().text;
        string password = RegisterPassword.GetComponent<TMP_InputField>().text;
        string confirm = ConfirmPassword.GetComponent<TMP_InputField>().text;

        if (username == string.Empty || password == string.Empty) return;
        if (confirm != password) return;

        Network.Enqueue(new C.NewAccount
        {
            AccountID = username,
            Password = password,
            EMailAddress = "na",
            BirthDate = DateTime.Now,
            UserName = "na",
            SecretQuestion = "na",
            SecretAnswer = "na"
        });
    }

    public void RegisterCancel_OnClick()
    {
        ClearRegisterBox();
        RegisterBox.SetActive(false);
        LoginBox.SetActive(true);
    }

    private void ClearRegisterBox()
    {
        RegisterUserName.GetComponent<TMP_InputField>().text = string.Empty;
        RegisterPassword.GetComponent<TMP_InputField>().text = string.Empty;
        ConfirmPassword.GetComponent<TMP_InputField>().text = string.Empty;
    }
}
