using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class CharSelManager : MonoBehaviour
{
    private List<SelectInfo> characters = new List<SelectInfo>();
    private GameObject loginCamera;
    private GameObject charselCamera;

    public LoginManager LoginManager;
    public MirButton[] CreateButtons = new MirButton[Globals.MaxCharacterCount];
    public MirSelectButton[] CharacterButtons = new MirSelectButton[Globals.MaxCharacterCount];
    public MirSelectButton[] ClassButtons = new MirSelectButton[Enum.GetNames(typeof(MirClass)).Length];
    public MirSelectButton[] GenderButtons = new MirSelectButton[Enum.GetNames(typeof(MirGender)).Length];
    public GameObject[] NewCharacterModels = new GameObject[Enum.GetNames(typeof(MirClass)).Length * Enum.GetNames(typeof(MirGender)).Length];
    public MirButton DeleteButton;
    public MirButton LogOutButton;
    public TMP_InputField NameInput;
    //Windows
    public GameObject SelectCharacterBox;
    public GameObject NewCharacterBox;
    //Misc
    public MirMessageBox MessageBox;
    public AudioSource audioSource;

    private SelectInfo selectedCharacter;
    private MirClass selectedClass;
    private MirGender selectedGender;
    private GameObject selectedModel;
    private GameObject activeLocation;
    private GameObject inactiveLocation;

    void Start()
    {
        activeLocation = GameObject.Find("ActiveLocation");
        inactiveLocation = GameObject.Find("InactiveLocation");
        loginCamera = GameObject.Find("LoginCamera");
        charselCamera = GameObject.Find("CharSelCamera");
        Network.CharSelManager = this;
    }

    public void OnLoaded()
    {
        audioSource.Play();
        SelectCharacterBox.SetActive(true);
        LogOutButton.gameObject.SetActive(true);
        GameManager.gameStage = GameStage.Select;
        Network.Enqueue(new C.RequestCharacters { });
    }

    public void ClearCreateBox()
    {
        ClassButtons[0].Select(true);
        GenderButtons[0].Select(true);
        NameInput.text = string.Empty;
        if (selectedModel != null)
            selectedModel.transform.SetPositionAndRotation(inactiveLocation.transform.position, inactiveLocation.transform.rotation);
        selectedModel = null;
    }

    public void ShowMessageBox(string message)
    {
        MessageBox.Show(message);
    }

    public void Create_Click()
    {
        if (NameInput.text.Length < 5)
        {
            ShowMessageBox("Name must be minimum 5 characters");
            return;
        }        

        Network.Enqueue(new C.NewCharacter
        {
            Name = NameInput.text,
            Class = selectedClass,
            Gender = selectedGender
        });
    }

    public void Refresh()
    {
        for (int i = 0; i < Globals.MaxCharacterCount; i++)
        {
            if (i >= characters.Count)
            {
                CreateButtons[i].gameObject.SetActive(true);
                CharacterButtons[i].gameObject.SetActive(false);
            }
            else
            {
                SelectInfo info = characters[i];
                CreateButtons[i].gameObject.SetActive(false);
                CharacterButtons[i].gameObject.SetActive(true);
                CharacterButtons[i].NeutralImage = Resources.Load<Sprite>($"UI/CharSel/{(byte)info.Class}_{(byte)info.Gender}_1");
                CharacterButtons[i].HoverImage = Resources.Load<Sprite>($"UI/CharSel/{(byte)info.Class}_{(byte)info.Gender}_2");
                CharacterButtons[i].DownImage = Resources.Load<Sprite>($"UI/CharSel/{(byte)info.Class}_{(byte)info.Gender}_1");
                CharacterButtons[i].SelectImage = Resources.Load<Sprite>($"UI/CharSel/{(byte)info.Class}_{(byte)info.Gender}_3");
                CharacterButtons[i].gameObject.transform.Find("Username").GetComponent<TextMeshProUGUI>().text = info.Name;
                CharacterButtons[i].gameObject.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = $"Level {info.Level}";
                CharacterButtons[i].gameObject.transform.Find("Class").GetComponent<TextMeshProUGUI>().text = info.Class.ToString();

                if (selectedCharacter == null)
                {
                    selectedCharacter = info;
                    CharacterButtons[i].Select(true);
                }
                CharacterButtons[i].gameObject.GetComponent<Image>().sprite = CharacterButtons[i].GetNeutralButton();
            }
        }

        DeleteButton.gameObject.SetActive(characters.Any(x => x != null));
    }

    public void RefreshModel()
    {
        if (selectedModel != null)
        {
            selectedModel.transform.SetPositionAndRotation(inactiveLocation.transform.position, inactiveLocation.transform.rotation);
            selectedModel.GetComponent<Animator>().SetBool("selected", false);
            selectedModel.GetComponent<Animator>().SetBool("bored", false);
            selectedModel.GetComponent<AudioSource>().Stop();
        }

        selectedModel = NewCharacterModels[(byte)selectedClass * 2 + (byte)selectedGender];
        selectedModel.transform.SetPositionAndRotation(activeLocation.transform.position, activeLocation.transform.rotation);
        selectedModel.GetComponent<Animator>().SetBool("bored", false);
        selectedModel.GetComponent<Animator>().SetBool("selected", true); 
    }

    public void NewCharacterSuccess(SelectInfo info)
    {
        AddCharacter(info);
        NewCharacterBox.SetActive(false);
        SelectCharacterBox.SetActive(true);
    }

    public void AddCharacters(List<SelectInfo> infos)
    {
        characters = infos;
        Refresh();
    }

    public void AddCharacter(SelectInfo info)
    {
        if (characters.Count >= Globals.MaxCharacterCount) return;
        characters.Add(info);
        Refresh();
    }

    public void SelectCharacter(int i)
    {
        if (characters[i] == null) return;

        selectedCharacter = characters[i];

        for (int j = 0; j < CharacterButtons.Length; j++)
            CharacterButtons[j].Select(i == j);
    }

    public void SelectClass(int i)
    {
        selectedClass = (MirClass)i;

        RefreshModel();

        for (int j = 0; j < ClassButtons.Length; j++)
            ClassButtons[j].Select(i == j);
    }

    public void SelectGender(int i)
    {
        selectedGender = (MirGender)i;

        RefreshModel();

        for (int j = 0; j < GenderButtons.Length; j++)
            GenderButtons[j].Select(i == j);
    }

    public void DeleteCharacter_OnClick()
    {
        if (selectedCharacter == null) return;

        MessageBox.Show($"Delete {selectedCharacter.Name}?", true, true);
        MessageBox.OK += () => 
        {
            Network.Enqueue(new C.DeleteCharacter() { CharacterIndex = selectedCharacter.Index });
        };
    }

    public void DeleteCharacterSuccess(int index)
    {
        selectedCharacter = null;
        characters.RemoveAll(x => x.Index == index);
        Refresh();
    }

    public void LogoutButton_OnClick()
    {
        MessageBox.Show($"Return to Login?", true, true);
        MessageBox.OK += () =>
        {
            Network.Enqueue(new C.Logout() { });
        };       
    }

    public void LogoutSuccess()
    {
        GameManager.gameStage = GameStage.Login;
        ChangeScene(GameManager.gameStage);
    }

    void ChangeScene(GameStage stage)
    {
        charselCamera.GetComponent<CameraFade>().Reset();
        switch (stage)
        {
            case GameStage.Login:
                charselCamera.GetComponent<Camera>().enabled = false;
                charselCamera.GetComponent<CameraFade>().enabled = false;
                charselCamera.GetComponent<AudioListener>().enabled = false;
                loginCamera.GetComponent<Camera>().enabled = true;
                loginCamera.GetComponent<CameraFade>().Reset();
                loginCamera.GetComponent<CameraFade>().enabled = false;
                loginCamera.GetComponent<AudioListener>().enabled = true;
                audioSource.Stop();
                LoginManager.OnLoaded();
                break;
        }        
    }
}
