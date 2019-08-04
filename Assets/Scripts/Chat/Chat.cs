using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using GeometryBattles.MenuUI;

public class Chat : MonoBehaviourPunCallbacks, IChatClientListener
{
    [SerializeField] private uint   historyBufferLength;
    [SerializeField] private string appVersion;
    [SerializeField] TMP_Dropdown   regionDropdown;
    //[SerializeField] TMP_Text       currentChannelText;
    [SerializeField] TMP_InputField chatInputField;
    
    public string username;

    protected internal AppSettings chatAppSettings;

    private string     _currentChannelName;
    private string     _roomName;
    private ChatClient _chatClient;
    private int        _region;
    private ChatBox    _chatBox;

    // Start is called before the first frame update
   private void Start()
   {
      _chatBox = GetComponent<ChatBox>();
      _chatBox.NewChatMessage += OnChatMessage;

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

   void OnChatMessage(object sender, NewChatMessageEventArgs e)
   {
      //Debug.Log($"Message sent from {e.nickName}: {e.message}");
      //chatBox.AddMessage("TEST", "Message Received", Color.black);
      SendChatMessage(e.message);
   }


   public override void OnJoinedRoom()
   {
      _roomName = PhotonNetwork.CurrentRoom.Name;
      _currentChannelName = _roomName.Trim();
      _chatClient.Subscribe(new string[] {_currentChannelName});
      Debug.Log($"Subscribed to {_currentChannelName}");
   }

   public override void OnLeftRoom()
   {
      _chatClient.Unsubscribe(new string[] {_roomName});
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
        _chatBox.UserNickName = username;
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
        Debug.Log($"Firing OnEnterSend() msg = {chatInputField.text}");
        //if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        //{
            SendChatMessage(chatInputField.text);
            chatInputField.text = "";
        //}
    }

    public void OnClickSend()
    {
            SendChatMessage(chatInputField.text);
            chatInputField.text = "";
    }

    private void SendChatMessage(string msg)
    {
       Debug.Log($"Calling SendChatMessage w/ msg = {msg}");

       if (msg is null || msg.Length == 0) return;

       Debug.Log($"About to publish message to {_currentChannelName}");
       _chatClient.PublishMessage(_currentChannelName, msg);
    }

   public void ShowChannelText(string channelName)
   {
      Debug.Log("Showing channel text");
         /* If no channel name was passed in or a channel by the given name cannot 
            be found, then return.
          */
      if (string.IsNullOrEmpty(channelName)) return;

      ChatChannel channel;
      bool channelFound = _chatClient.TryGetChannel(channelName, out channel);

      if (!channelFound)
      {
         Debug.Log($"Could not find channel {channelName}");
         return;
      }

      //currentChannelText.text = channel.ToStringMessages();
      _chatBox.AddMessage("", channel.ToStringMessages(), Color.white);
      Debug.Log(channel.ToStringMessages());
   }


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
      if (channelName.Equals(_currentChannelName))
      {
         ShowChannelText(channelName);
      }
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
      Debug.Log("Calling OnSubscribed");
      foreach (string channel in channels)
      {
         Debug.Log($"subscribed to channel: {channel}");
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
