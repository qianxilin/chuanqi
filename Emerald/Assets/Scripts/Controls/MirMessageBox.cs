using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MirMessageBox : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public GameObject OKButton;
    public GameObject CancelButton;
    public static MessageBoxResult Result;


    public void Show(string s, bool okbutton, bool cancelbutton)
    {
        Text.text = s;
        OKButton.SetActive(okbutton);
        CancelButton.gameObject.SetActive(cancelbutton);
        Result = MessageBoxResult.None;
        gameObject.SetActive(true);
    }

    public void OKButton_Click()
    {
        Result = MessageBoxResult.Ok;
        gameObject.SetActive(false);
    }

    public void CancelButton_Click()
    {
        Result = MessageBoxResult.Cancel;
        gameObject.SetActive(false);
    }
}
