using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;

public class Chat : MonoBehaviour, IChatClientListener
{
    [SerializeField] private uint   historyBufferLength;
    [SerializeField] private string appVersion;
    [SerializeField] TMP_Dropdown   regionDropdown;
    
    public string   username;
    public TMP_Text currentChannelText;

    protected internal AppSettings chatAppSettings;

    private string     _currentChannelName;
    private ChatClient _chatClient;
    private int        _region;

    // Start is called before the first frame update
   private void Start()
   {
         // We want to be able to chat in subsequent scenes
      Transform managersTransform = gameObject.transform;

      while (!(managersTransform.parent is null))
      {
         managersTransform = managersTransform.parent;
      }

      DontDestroyOnLoad(managersTransform.gameObject);

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
    private void Update()
    {
            // Keep an active connection alive and process incoming messages
        _chatClient?.Service();
    }

   public void Connect()
   {
      _chatClient = new ChatClient(this);
      _region = regionDropdown.value;

      if (_region <= 2 || _region == 5 || _region == 11)
      {
        _chatClient.ChatRegion = "US";
      }
      else if (_region == 6  || _region == 7)
      {
         _chatClient.ChatRegion = "EU";
      }
      else
      {
          _chatClient.ChatRegion = "ASIA";
      }

      Debug.Log($"Connecting to {_chatClient.ChatRegion} Photon Chat region");

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

    public void OnRegionChanged()
    {
       _region = regionDropdown.value;
    }

    private void OnDestroy()
    {
        _chatClient?.Disconnect();
    }

    private void OnApplicationQuit()
    {
        _chatClient?.Disconnect();
    }

    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            // SendChatMessage(chatInputField.text);
            // chatInputField.text = "";
        }
    }

    public void OnClickSend()
    {
        //if (!(chatInputField is null))
        {
            // SendChatMessage(chatInputField.text);
            // chatInputField.text = "";
        }
    }

    // Add methods to subscribe when a room is joined and unsubscribe when it's left

#region IChatClientListener implementation
   void IChatClientListener.DebugReturn(DebugLevel level, string message)
   {
      Debug.Log(message);
   }

   void IChatClientListener.OnDisconnected()
   {
      Debug.Log("IChatClientListener.OnDisconnected called");
   }

   void IChatClientListener.OnConnected()
   {
      Debug.Log($"Connected as {username}");
   }

   void IChatClientListener.OnChatStateChange(ChatState state)
   {
      Debug.Log($"state: {state.ToString()}");
   }

   void IChatClientListener.OnGetMessages(
      string channelName, 
      string[] senders, 
      object[] messages)
   {
      // Because all users will be in the same chat room, we don't need to worry
      // about a callback here.
      ;
   }

   void IChatClientListener.OnPrivateMessage(
       string sender,
       object message,
       string channelName)
   {
       // Private messages are not implemented in this chat system
      ;
   }

   void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
   {
      foreach (string channel in channels)
      {
          _chatClient.PublishMessage(channel, $"{username} has joined.");
      }
   }

   void IChatClientListener.OnUnsubscribed(string[] channels)
   {
      foreach (string channel in channels)
      {
          Debug.Log($"Unsibscribed from {channel}");
      }

      // Could implement a side bar w/ various subscribed channels and when a user
      // unsibscribes from a channel, that sidebar disappears.
   }

   void IChatClientListener.OnStatusUpdate(
      string user,
      int    status,
      bool   gotMessage,
      object message)
   {
      // Can implement later for incorportating some sort of friend list
      ;
   }

   void IChatClientListener.OnUserSubscribed(string channel, string user)
   {
      Debug.Log($"{user} subscribed from {channel}");
   }

   void IChatClientListener.OnUserUnsubscribed(string channel, string user)
   {
      Debug.Log($"{user} unsubscribed from {channel}");
   }

   #endregion
}
