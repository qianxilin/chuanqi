using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class CharSelManager : MonoBehaviour
{
    private SelectInfo[] characters = new SelectInfo[Globals.MaxCharacterCount];
    public MirSelectButton WarriorButton;
    public MirSelectButton WizardButton;
    public MirSelectButton TaoistButton;
    public MirSelectButton AssassinButton;
    public MirSelectButton ArcherButton;
    public MirSelectButton MaleButton;
    public TMP_InputField NameInput;

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
    }
}
