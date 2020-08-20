using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class GameSceneManager : MonoBehaviour
{
    public TMP_InputField ChatBar;      
    public Scrollbar ScrollBar;
    public ChatController ChatController;
    public EventSystem eventSystem;
    public TMP_Text CharacterName;
    public TMP_Text CharacterLevel;
    public Image CharacterIcon;
    public Sprite[] CharacterIcons = new Sprite[Enum.GetNames(typeof(MirClass)).Length * Enum.GetNames(typeof(MirGender)).Length];
    public Button AttackModeButton;
    public TMP_Text AttackModeText;
    public Sprite[] AttackModeIcons = new Sprite[Enum.GetNames(typeof(AttackMode)).Length];
    public Sprite[] AttackModeHoverIcons = new Sprite[Enum.GetNames(typeof(AttackMode)).Length];
    public Sprite[] AttackModeDownIcons = new Sprite[Enum.GetNames(typeof(AttackMode)).Length];

    void Awake()
    {
        GameManager.GameScene = this;             
    }

    void Start()
    {
        ScrollBar.size = 0.4f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ChatBar.gameObject.activeSelf)
            {
                
                if (ChatBar.text.Length > 0)
                    Network.Enqueue(new C.Chat() { Message = ChatBar.text });
                ChatBar.text = string.Empty;
                ChatBar.gameObject.SetActive(false);
            }
            else
            {
                ChatBar.gameObject.SetActive(true);
                ChatBar.Select();
            }
        }

        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !eventSystem.IsPointerOverGameObject())
        {
            GameManager.CheckMouseInput();
        }
    }

    public void UpdateCharacterIcon()
    {
        CharacterIcon.sprite = CharacterIcons[(int)GameManager.User.Player.Class * 2 + (int)GameManager.User.Player.Gender];
        CharacterName.text = GameManager.User.Player.name;
        CharacterLevel.text = GameManager.User.Level.ToString();
    }

    public void ChangeAttackMode(int amode)
    {
        if (amode >= Enum.GetNames(typeof(AttackMode)).Length) return;
        Network.Enqueue(new C.ChangeAMode() { Mode = (AttackMode)amode });
    }

    public void SetAttackMode(AttackMode amode)
    {
        AttackModeButton.GetComponent<Image>().sprite = AttackModeIcons[(int)amode];

        SpriteState state = new SpriteState();
        state = AttackModeButton.spriteState;
        state.highlightedSprite = AttackModeHoverIcons[(int)amode];
        state.pressedSprite = AttackModeDownIcons[(int)amode];

        AttackModeButton.spriteState = state;

        AttackModeText.text = amode.ToString();
    }


}
