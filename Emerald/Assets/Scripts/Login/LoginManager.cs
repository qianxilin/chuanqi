using System;
using UnityEngine;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class LoginManager : MonoBehaviour
{
    //Animations
    public Animator LeftDoor, RightDoor, Camera, DoorFX;
    public AudioSource DoorOpenSound;
    //Connection
    public GameObject ConnectBox;
    //Login
    public GameObject LoginBox;
    public TMP_InputField UserName;
    public TMP_InputField Password;
    //Register
    public GameObject RegisterBox;
    public TMP_InputField RegisterUserName;
    public TMP_InputField RegisterPassword;
    public TMP_InputField ConfirmPassword;
    //Change Password
    public GameObject ChangePasswordBox;
    public TMP_InputField ChangeUserName;
    public TMP_InputField ChangeCurrentPassword;
    public TMP_InputField ChangeNewPassword;
    public TMP_InputField ChangeConfirmPassword;
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
                if (UserName.isFocused)
                    Password.Select();
                else if (Password.isFocused)
                    UserName.Select();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                string username = UserName.text;
                string password = Password.text;

                if (username != string.Empty && password != string.Empty)
                    LoginButton_OnClick();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            LoginSuccess();
        }
    }

    public void ShowLoginBox()
    {
        loginshown = true;
        ConnectBox.SetActive(false);
        LoginBox.SetActive(true);
        UserName.ActivateInputField();
    }

    public void ShowMessageBox(string message)
    {
        MessageBox.GetComponentInChildren<TextMeshProUGUI>().text = message;
        MessageBox.SetActive(true);
    }

    public void LoginButton_OnClick()
    {
        string username = UserName.text;
        string password = Password.text;

        if (username == string.Empty || password == string.Empty) return;

        Network.Enqueue(new C.Login
        {
            AccountID = username,
            Password = password
        });

        UserName.text = string.Empty;
        Password.text = string.Empty;
    }

    public void RegisterButton_OnClick()
    {
        string username = RegisterUserName.text;
        string password = RegisterPassword.text;
        string confirm = ConfirmPassword.text;

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
        RegisterUserName.text = string.Empty;
        RegisterPassword.text = string.Empty;
        ConfirmPassword.text = string.Empty;
    }

    public void ChangePasswordButton_OnClick()
    {
        string username = ChangeUserName.text;
        string password = ChangeCurrentPassword.text;
        string newpassword = ChangeNewPassword.text;
        string confirm = ChangeConfirmPassword.text;

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
        ChangeUserName.text = string.Empty;
        ChangeCurrentPassword.text = string.Empty;
        ChangeNewPassword.text = string.Empty;
        ChangeConfirmPassword.text = string.Empty;
    }

    public void LoginSuccess()
    {
        DoorOpenSound.Play();
        LoginBox.SetActive(false);
        LeftDoor.SetBool("openGate", true);
        RightDoor.SetBool("openGate", true);
        Camera.SetBool("openGate", true);
        DoorFX.SetBool("openGate", true);
    }
}
