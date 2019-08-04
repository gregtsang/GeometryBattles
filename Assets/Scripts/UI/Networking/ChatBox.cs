using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace GeometryBattles.MenuUI
{
    [RequireComponent(typeof (Chat))]
    public class ChatBox : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField = null;
        [SerializeField] Transform messageContainer = null;
        [SerializeField] ScrollRect scrollView = null;
        
        [SerializeField] float msgSize = 20;
        [SerializeField] Color msgColor = Color.white;
        [SerializeField] string seperator = "  ";

        [SerializeField] int maxEntries = 5;

        private string userNickName = "erch";
        private Color userColor = Color.blue;

        public string UserNickName { get => userNickName; set => userNickName = value; }
        public Color UserColor { get => userColor; set => userColor = value; }

        public event EventHandler<NewChatMessageEventArgs> NewChatMessage;

        void Start()
        {
            if (inputField != null)
            {
                inputField.onSubmit.AddListener(delegate
                {
                    AddMessage(userNickName, inputField.text, userColor);
                    RaiseNewChatMessageEvent(userNickName, inputField.text);
                    ResetInputField();
                });
            }
        }

        public void AddMessage(string nickName, string message, Color color)
        {
            if (message == "") return;

            CreateChatEntry(nickName, message, color).transform.SetParent(messageContainer);
            CheckMaxEntries();
        }

        private void RaiseNewChatMessageEvent(string userNickName, string text)
        {
            NewChatMessageEventArgs e = new NewChatMessageEventArgs();
            e.nickName = userNickName;
            e.message = text;
            OnNewChatMessage(e);
        }

        private void ResetInputField()
        {
            inputField.text = "";
            inputField.ActivateInputField();
            if (scrollView.verticalNormalizedPosition <= 0.0001f)
            {
                MoveScrollViewToBottom();
            }
        }

        private void MoveScrollViewToBottom()
        {
            Canvas.ForceUpdateCanvases();
            scrollView.normalizedPosition = new Vector2(0, 0);
        }

        private void CheckMaxEntries()
        {
            if (messageContainer.childCount > maxEntries)
            {
                Destroy(messageContainer.transform.GetChild(0).gameObject);
            }
        }

        private GameObject CreateChatEntry(string nickName, string message, Color nameColor)
        {
            GameObject newGameObject = new GameObject();
            TextMeshProUGUI text = newGameObject.AddComponent<TextMeshProUGUI>();
            text.text = CreateNameText(nickName, nameColor);
            text.text += CreateMessageText(message, msgColor);
            text.fontSize = msgSize;
            text.autoSizeTextContainer = true;
            return newGameObject;
        }

        private string CreateNameText(string name, Color color)
        {
            string nameText = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
            nameText += $"<b>{name}</b></color>";
            return nameText;
        }

        private string CreateMessageText(string msg, Color color)
        {
            string msgText = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
            msgText += $"{seperator}{msg}</color>";
            return msgText;
        }

        protected virtual void OnNewChatMessage(NewChatMessageEventArgs e)
        {
            EventHandler<NewChatMessageEventArgs> handler = NewChatMessage;
            if (NewChatMessage != null)
            {
                NewChatMessage(this, e);
            }
        }
    }

    public class NewChatMessageEventArgs : EventArgs
    {
        public string nickName { get; set; }
        public string message { get; set; }
    }
}