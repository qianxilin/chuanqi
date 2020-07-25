using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network = EmeraldNetwork.Network;

public class LoginManager : MonoBehaviour
{
    //Login
    public GameObject LoginBox;
    public GameObject RegisterBox;

    private bool loginshown = false;

    // Start is called before the first frame update
    void Start()
    {
        LoginBox.SetActive(false);
        RegisterBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!loginshown && Network.LoginConnected)
            ShowLoginBox();
    }

    public void ShowLoginBox()
    {
        loginshown = true;
        LoginBox.SetActive(true);
        RegisterBox.SetActive(true);
    }
}
