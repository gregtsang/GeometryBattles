using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ReadyChecker : MonoBehaviourPunCallbacks
{
    
    [SerializeField] bool requireMaxPlayers = true;
    [SerializeField] int countdownLength = 5;
    [SerializeField] TextMeshProUGUI countdownText = null;

    Coroutine countdownCoroutine = null;

    override public void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(){
            {"ready", false}
        });
        RPC_StopCountdown();
    }
    
    override public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.photonView.RPC("RPC_StopCountdown", RpcTarget.AllViaServer);
        }
    }

    override public void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (changedProps.ContainsKey("ready"))
            {
                CheckAndStartGame();
            }
        }
    }

    private void CheckAndStartGame()
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room == null) return;
        if ((room.PlayerCount == room.MaxPlayers || !requireMaxPlayers) && CheckAllPlayersReady())
        {
            this.photonView.RPC("RPC_StartCountdown", RpcTarget.AllViaServer);
        }
        else
        {
            this.photonView.RPC("RPC_StopCountdown", RpcTarget.AllViaServer);
        }
    }

    IEnumerator CountDownToStart(int countdownLength)
    {
        for (int i = countdownLength; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("STARTING GAME...");
            PhotonNetwork.LoadLevel(1);;
        }
    }

    [PunRPC]
    void RPC_StartCountdown()
    {
        countdownCoroutine = StartCoroutine(CountDownToStart(countdownLength));
    }

    [PunRPC]
    void RPC_StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            countdownText.text = "";
            StopCoroutine(countdownCoroutine);
        }
    }

    bool CheckAllPlayersReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!CheckPlayerReady(player))
            {
                return false;
            }
        }
        return true;
    }

    bool CheckPlayerReady(Player player)
    {
        object readyObject;
        if (player.CustomProperties.TryGetValue("ready", out readyObject))
        {
            return (bool) readyObject;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
