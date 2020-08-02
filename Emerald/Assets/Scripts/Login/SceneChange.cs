using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    void ChangeScene()
    {
        SceneManager.LoadSceneAsync("CharSel");             
    }
}
