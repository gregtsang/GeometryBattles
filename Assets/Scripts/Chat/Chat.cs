using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Chat : MonoBehaviour, IChatClientListener
{
    public string username = "unpicked_username";

    [SerializeField]
    private uint _historyBufferLength;

    [SerializeField]
    private string appVersion;

    private ChatClient _chatClient;

    protected internal AppSettings chatAppSettings;


    // Start is called before the first frame update
    void Start()
    {
            // We want to be able to chat in subsequent scenes
            // TODO - warning on this call 
        DontDestroyOnLoad(this.gameObject);

            // Determine if the application has an appid for Photon Chat
        chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
        if (chatAppSettings.AppIdChat is null)
        {
            Debug.Log("Missing appid for Photon Chat");
        }
        else
        {
            Debug.Log($"Photon Chat appid: {chatAppSettings.AppIdChat}");
        }
    }

    // Update is called once per frame
    void Update()
    {
            // Keep an active connection alive and process incoming messages
        _chatClient?.Service();
    }

    public void Connect()
    {
        _chatClient = new ChatClient(this);

        #if UNITY_WEBGL
            _chatClient.UseBackgroundWorkerForSending = false;
        #else
            _chatClient.UseBackgroundWorkerForSending = true;
        #endif

        _chatClient.Connect(
            chatAppSettings.AppIdChat, 
            appVersion,
            new Photon.Chat.AuthenticationValues(username)
        );

        Debug.Log($"Connecting to Photon Chat servers as {username}");
    }

    private void OnDestroy()
    {
        _chatClient?.Disconnect();
    }

    private void OnApplicationQuit()
    {
        _chatClient?.Disconnect();
    }

#region IChatClientListener implementation
   void IChatClientListener.DebugReturn(DebugLevel level, string message)
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnDisconnected()
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnConnected()
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnChatStateChange(ChatState state)
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnUnsubscribed(string[] channels)
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnUserSubscribed(string channel, string user)
   {
      throw new System.NotImplementedException();
   }

   void IChatClientListener.OnUserUnsubscribed(string channel, string user)
   {
      throw new System.NotImplementedException();
   }

   #endregion
}
