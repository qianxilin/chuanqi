using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Network = EmeraldNetwork.Network;

public class LoginConnectBox : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = $"Connecting attempt: {Network.ConnectAttempt}";
    }
}
