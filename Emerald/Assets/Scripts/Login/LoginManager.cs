using System;
using UnityEngine;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class LoginManager : MonoBehaviour
{
    //Animations
    public Animator LeftDoor, RightDoor, Camera;
    public AudioSource DoorOpenSound;
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
    //Change Password
    public GameObject ChangePasswordBox;
    public GameObject ChangeUserName;
    public GameObject ChangeCurrentPassword;
    public GameObject ChangeNewPassword;
    public GameObject ChangeConfirmPassword;
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
            Network.LoginManager = this;
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
            EMailAddress = "na@na.com",
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

    public void ChangePasswordButton_OnClick()
    {
        string username = ChangeUserName.GetComponent<TMP_InputField>().text;
        string password = ChangeCurrentPassword.GetComponent<TMP_InputField>().text;
        string newpassword = ChangeNewPassword.GetComponent<TMP_InputField>().text;
        string confirm = ChangeConfirmPassword.GetComponent<TMP_InputField>().text;

        if (username == string.Empty || password == string.Empty || newpassword == string.Empty) return;
        if (confirm != newpassword) return;

        Network.Enqueue(new C.ChangePassword
        {
            AccountID = username,
            CurrentPassword = password,
            NewPassword = newpassword
        });
    }

    public void ChangeCancel_OnClick()
    {
        ClearChangeBox();
        ChangePasswordBox.SetActive(false);
        LoginBox.SetActive(true);
    }

    private void ClearChangeBox()
    {
        ChangeUserName.GetComponent<TMP_InputField>().text = string.Empty;
        ChangeCurrentPassword.GetComponent<TMP_InputField>().text = string.Empty;
        ChangeNewPassword.GetComponent<TMP_InputField>().text = string.Empty;
        ChangeConfirmPassword.GetComponent<TMP_InputField>().text = string.Empty;
    }

    public void LoginSuccess()
    {
        DoorOpenSound.Play();
        LoginBox.SetActive(false);
        LeftDoor.SetBool("openGate", true);
        RightDoor.SetBool("openGate", true);
        Camera.SetBool("openGate", true);
    }
}
