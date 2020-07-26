using System;
using UnityEngine;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class LogInHandler : MonoBehaviour
{
    public GameObject UserName;
    public GameObject Password;

    public void OnClick()
    {
        Debug.Log("Click");
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
}
