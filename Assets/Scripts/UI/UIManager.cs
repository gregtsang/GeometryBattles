using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;
using GeometryBattles.BoardManager;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System;

namespace GeometryBattles.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private int activePlayer = 0;
        [SerializeField] GameObject hexActionCanvasPrefab = null;
        [SerializeField] GameObject hexActionOptionPrefab = null;
        
        private Board board;
        private GameObject buttonGroup;

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

            CreateHexActionMenuPrefab();
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

        public void ShowHexActionMenu()
        {
            buttonGroup.transform.position = Input.mousePosition;
            buttonGroup.SetActive(true);

        }

        public void HideHexActionMenu()
        {
            buttonGroup.SetActive(false);
        }

        public void InitializeHexActionMenu()
        {
            foreach (Transform child in buttonGroup.gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public Button AddActionToHexActionMenu(string actionText)
        {
            GameObject newButton = Instantiate(hexActionOptionPrefab);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = actionText;
            newButton.transform.SetParent(buttonGroup.gameObject.transform);
            return newButton.GetComponent<Button>();
        }
        
        private void CreateHexActionMenuPrefab()
        {
            GameObject newCanvas = Instantiate(hexActionCanvasPrefab);
            newCanvas.transform.SetParent(gameObject.transform);
            buttonGroup = newCanvas.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            HideHexActionMenu();
        }
    }
}