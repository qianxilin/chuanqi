using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class MirMessageBox : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public GameObject OKButton;
    public GameObject CancelButton;
    public static MessageBoxResult Result;

    [HideInInspector]
    public delegate void OKDelegate();
    public OKDelegate OK;
    public delegate void CancelDelegate();
    public OKDelegate Cancel;


    public async void Show(string s, bool okbutton = true, bool cancelbutton = false)
    {
        Text.text = s;
        OKButton.SetActive(okbutton);
        CancelButton.gameObject.SetActive(cancelbutton);
        Result = MessageBoxResult.None;
        gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
        OK = null;
        Cancel = null;

        while (Result == MessageBoxResult.None)
        {
            await Task.Yield();
        }

        switch (Result)
        {
            case MessageBoxResult.Ok:
                OK?.Invoke();
                break;
            case MessageBoxResult.Cancel:
                Cancel?.Invoke();
                break;
        }
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
