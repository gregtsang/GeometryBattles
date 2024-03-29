﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;
using GeometryBattles.BoardManager;
using GeometryBattles.HexAction;
using Photon.Pun;

namespace GeometryBattles.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] HexSelectionManager hexSelectionManager = null;
        [SerializeField] private int activePlayer = 0;
        
        public HexSelectionManager HexSelectionManager { get => hexSelectionManager; set => hexSelectionManager = value; }
        
        private Board board;
        private HexActionModeManager modeManager;

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
            modeManager = FindObjectOfType<HexActionModeManager>();
            modeManager.ModeChanged += ActionModeChanged;
        }

        public Player GetActivePlayer()
        {
            if (board == null) return null;
            Player player = board.boardState.GetPlayer(activePlayer);
            if (player == null) return null;
            return player;
        }

        public string GetPlayerUserName(int playerID)
        {
            return PhotonNetwork.PlayerList[playerID].NickName;
        }

        public Board GetBoard()
        {
            return board == null ? FindObjectOfType<Board>() : board;
        }

        private void ActionModeChanged(object sender, ModeChangedEventArgs e)
        {
            hexSelectionManager.HoverHighlightColor = e.newMode.HoverColor;
            hexSelectionManager.HoverHighlightSize = e.newMode.HoverSize;
        }
    }
}