using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;
using GeometryBattles.BoardManager;

namespace GeometryBattles.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private int activePlayer = 0;
        
        private Board board;
        public Board Board { get => board; }

        // Start is called before the first frame update
        void Start()
        {
            board = FindObjectOfType<Board>();
        }

        public PlayerPrefab GetActivePlayer()
        {
            return Board.boardState.GetPlayer(activePlayer).GetComponent<PlayerPrefab>();
        }

        public Board GetBoard()
        {
            return board;
        }
    }
}