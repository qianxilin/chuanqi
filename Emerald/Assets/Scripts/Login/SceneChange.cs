using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private GameObject loginCamera;
    private GameObject charselCamera;
    public CharSelManager CharselManager;

    void Start()
    {
        loginCamera = GameObject.Find("LoginCamera");
        charselCamera = GameObject.Find("CharSelCamera");
        charselCamera.GetComponent<Camera>().enabled = false;
    }
    void ChangeScene()
    {
        loginCamera.GetComponent<Camera>().enabled = false;
        loginCamera.GetComponent<CameraFade>().enabled = false;
        loginCamera.GetComponent<AudioListener>().enabled = false;
        charselCamera.GetComponent<Camera>().enabled = true;        
        charselCamera.GetComponent<CameraFade>().enabled = true;
        charselCamera.GetComponent<AudioListener>().enabled = true;

        CharselManager.OnLoaded();
    }

    void CameraFadeOut()
    {
        GetComponent<CameraFade>().enabled = true;
    }
}
