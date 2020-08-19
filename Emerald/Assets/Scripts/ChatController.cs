using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatController : MonoBehaviour
{
    const int MaxChatMessages = 200;
    public GameObject ChatPanel;
    public List<ChatMessage> chatMessages = new List<ChatMessage>();
    public GameObject TextObject;

    private bool[] Filter = new bool[Enum.GetNames(typeof(ChatType)).Length];

    public GameObject[] FilterObjects = new GameObject[7];

    public void RecieveChat(string text, ChatType type)
    {
        ChatMessageBody cm = new ChatMessageBody();
        cm.text = text;
        cm.type = type;

        ChatMessage newText = Instantiate(TextObject, ChatPanel.transform).GetComponent<ChatMessage>();
        newText.Info = cm;

        chatMessages.Add(newText);

        if (chatMessages.Count > MaxChatMessages)
        {
            Destroy(chatMessages[0].gameObject);
            chatMessages.RemoveAt(0);
        }
    }

    public void ToggleFilter(int type)
    {
    }
}
