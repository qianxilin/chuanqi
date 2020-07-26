using System.Collections;
using System.Collections.Generic;
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
    public GameObject Email;

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

    /*public void RegisterButton_OnClick()
    {
        string username = UserName.GetComponent<InputField>().text;
        string email = Email.GetComponent<InputField>().text;
        string password = Password.GetComponent<InputField>().text;
        string confirm = ConfirmPassword.GetComponent<InputField>().text;

        if (username == string.Empty || password == string.Empty) return;
        if (confirm != password) return;

        Network.Enqueue(new C.NewAccount
        {
            AccountID = username,
            Password = password,
            EMailAddress = email,
            BirthDate = DateTime.Now,
            UserName = "na",
            SecretQuestion = "na",
            SecretAnswer = "na"
        });

        UserName.GetComponent<InputField>().text = string.Empty;
        Email.GetComponent<InputField>().text = string.Empty;
        Password.GetComponent<InputField>().text = string.Empty;
        ConfirmPassword.GetComponent<InputField>().text = string.Empty;
    }*/
}
