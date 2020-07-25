using System;
using UnityEngine;
using UnityEngine.UI;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class LogInHandler : MonoBehaviour
{
    public GameObject UserName;
    public GameObject Password;

    public void OnClick()
    {
        string username = UserName.GetComponent<InputField>().text;
        string password = Password.GetComponent<InputField>().text;

        if (username == string.Empty || password == string.Empty) return;

        Network.Enqueue(new C.Login
        {
            AccountID = username,
            Password = password
        });

        UserName.GetComponent<InputField>().text = string.Empty;
        Password.GetComponent<InputField>().text = string.Empty;
    }
}
