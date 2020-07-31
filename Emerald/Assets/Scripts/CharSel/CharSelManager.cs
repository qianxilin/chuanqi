using System.Collections;
using System.Collections.Generic;
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
    public MirSelectButton WarriorButton;
    public MirSelectButton WizardButton;
    public MirSelectButton TaoistButton;
    public MirSelectButton AssassinButton;
    public MirSelectButton ArcherButton;
    public MirSelectButton MaleButton;
    public TMP_InputField NameInput;
    //Misc
    public GameObject MessageBox;

    void Start()
    {
        GameManager.gameStage = GameStage.Select;
        Network.CharSelManager = this;
        Network.Enqueue(new C.RequestCharacters{});
    }

    public void ClearCreateBox()
    {
        WarriorButton.Select(true);
        MaleButton.Select(true);
        NameInput.text = string.Empty;
    }

    public void ShowMessageBox(string message)
    {
        MessageBox.GetComponentInChildren<TextMeshProUGUI>().text = message;
        MessageBox.SetActive(true);
    }

    public void Create_Click()
    {
        if (NameInput.text.Length < 5) return;

        MirClass newclass = MirClass.Warrior;
        MirGender newgender = MirGender.Male;

        if (WizardButton.Selected)
            newclass = MirClass.Wizard;
        if (TaoistButton.Selected)
            newclass = MirClass.Taoist;
        if (AssassinButton.Selected)
            newclass = MirClass.Assassin;
        if (ArcherButton.Selected)
            newclass = MirClass.Archer;

        if (!MaleButton.Selected)
            newgender = MirGender.Female;

        Network.Enqueue(new C.NewCharacter
        {
            Name = NameInput.text,
            Class = newclass,
            Gender = newgender
        });
    }

    public void NewCharacterSuccess(SelectInfo info)
    {
        AddCharacter(info);
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
            CharacterButtons[i].gameObject.GetComponent<Image>().sprite = CharacterButtons[i].GetNeutralButton();

            CharacterButtons[i].gameObject.transform.Find("Username").GetComponent<TextMeshProUGUI>().text = info.Name;
            CharacterButtons[i].gameObject.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = $"Level {info.Level}";
            CharacterButtons[i].gameObject.transform.Find("Class").GetComponent<TextMeshProUGUI>().text = info.Class.ToString();
            break;
        }
    }

}
