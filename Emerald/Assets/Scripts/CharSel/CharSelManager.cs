using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class CharSelManager : MonoBehaviour
{
    private SelectInfo[] characters = new SelectInfo[Globals.MaxCharacterCount];
    public MirButton[] CreateButtons = new MirButton[Globals.MaxCharacterCount];
    public MirSelectButton[] CharacterButtons = new MirSelectButton[Globals.MaxCharacterCount];
    public MirSelectButton[] ClassButtons = new MirSelectButton[Enum.GetNames(typeof(MirClass)).Length];
    public MirSelectButton[] GenderButtons = new MirSelectButton[Enum.GetNames(typeof(MirGender)).Length];
    public MirButton DeleteButton;
    public TMP_InputField NameInput;
    //Windows
    public GameObject SelectCharacterBox;
    public GameObject NewCharacterBox;
    //Misc
    public MirMessageBox MessageBox;

    private SelectInfo selectedCharacter;
    private MirClass selectedClass;
    private MirGender selectedGender;

    void Start()
    {
        GameManager.gameStage = GameStage.Select;
        Network.CharSelManager = this;
        Network.Enqueue(new C.RequestCharacters{});
    }

    public void ClearCreateBox()
    {
        ClassButtons[0].Select(true);
        GenderButtons[0].Select(true);
        NameInput.text = string.Empty;
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
        DeleteButton.gameObject.SetActive(characters.Any(x => x != null));
    }

    public void NewCharacterSuccess(SelectInfo info)
    {
        AddCharacter(info);
        NewCharacterBox.SetActive(false);
        SelectCharacterBox.SetActive(true);
    }

    public void AddCharacter(SelectInfo info)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null) continue;
            characters[i] = info;
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

            
            break;
        }
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

        for (int j = 0; j < ClassButtons.Length; j++)
            ClassButtons[j].Select(i == j);
    }

    public void SelectGender(int i)
    {
        selectedGender = (MirGender)i;

        for (int j = 0; j < GenderButtons.Length; j++)
            GenderButtons[j].Select(i == j);
    }

    public void DeleteCharacter_OnClick()
    {
        if (selectedCharacter == null) return;

        MessageBox.Show($"Delete {selectedCharacter.Name}?", true, true);
        MessageBox.OK += () => { Debug.Log("Delete"); };
    }

}
