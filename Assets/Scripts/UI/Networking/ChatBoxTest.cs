using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.MenuUI
{
    public class ChatBoxTest : MonoBehaviour
    {
        ChatBox chatBox;
        
        // Start is called before the first frame update
        void Start()
        {
            chatBox = GetComponent<ChatBox>();
            chatBox.NewChatMessage += OnChatMessage;
        }

        void OnChatMessage(object sender, NewChatMessageEventArgs e)
        {
            Debug.Log($"Message sent from {e.nickName}: {e.message}");
            chatBox.AddMessage("TEST", "Message Received", Color.black);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}