using System;
using UnityEngine;
using UnityEngine.UI;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class RegisterHandler : MonoBehaviour
{
    public GameObject UserName;
    public GameObject Email;
    public GameObject Password;
    public GameObject ConfirmPassword;

    public void OnClick()
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
    }
}
