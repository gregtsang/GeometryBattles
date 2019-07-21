using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;
using GeometryBattles.BoardManager;
using Photon.Pun;

namespace GeometryBattles.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private int activePlayer = 0;
        
        private Board board;

        private void Awake()
        {
            for (int i = 0; i != PhotonNetwork.PlayerList.Length; ++i)
            {
                if (PhotonNetwork.PlayerList[i].IsLocal)
                {
                    activePlayer = i;
                }
            }

            Debug.Log("I am player " + activePlayer);
        }

      // Start is called before the first frame update
      void Start()
        {
            board = FindObjectOfType<Board>();
        }

        public Player GetActivePlayer()
        {
            if (board == null) return null;
            Player player = board.boardState.GetPlayer(activePlayer);
            if (player == null) return null;
            return player;
        }

        public Board GetBoard()
        {
            return board;
        }
    }
}