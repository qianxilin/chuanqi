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
    public GameObject ChatPanel;
    public GameObject TextObject;
    public Scrollbar ScrollBar;
    public List<ChatMessage> chatMessages = new List<ChatMessage>();
    public EventSystem eventSystem;

    const int MaxChatMessages = 200;

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

        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && eventSystem.currentSelectedGameObject == null)
        {
            GameManager.CheckMouseInput();
        }
    }

    public void RecieveChat(string text)
    {
        ChatMessage cm = new ChatMessage();
        cm.text = text;
        GameObject newText = Instantiate(TextObject, ChatPanel.transform);
        cm.textObject = newText.GetComponent<TMP_Text>();
        cm.textObject.text = text;
        chatMessages.Add(cm);

        if (chatMessages.Count > MaxChatMessages)
        {
            Destroy(chatMessages[0].textObject.gameObject);
            chatMessages.RemoveAt(0);
        }
    }
}

public class ChatMessage
{
    public string text;
    public TMP_Text textObject;
}
