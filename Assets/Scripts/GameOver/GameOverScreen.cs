using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeometryBattles.PlayerManager;
using TMPro;

namespace GeometryBattles.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        [Header("GUI Objects")]
        [SerializeField] GameObject gameOverCanvas = null;
        [SerializeField] TextMeshProUGUI winnerText = null;
        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] Image imageContainer = null;
        [Header("Patameters")]
        [SerializeField] string winText = "YOU WIN!!";
        [SerializeField] string loseText = "YOU LOSE!!";
        [SerializeField] Color winColor = Color.white;
        [SerializeField] Color loseColor = Color.red;

        UIManager uiManager;

        void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
            gameOverCanvas.SetActive(false);

            EventManager.onGameOver += GameOver;
        }

        private void GameOver(GameObject winner)
        {
            gameOverCanvas.SetActive(true);
            
            Player activePlayer = uiManager.GetActivePlayer();
            Player winningPlayer = winner.GetComponent<Player>();

            if (activePlayer == winningPlayer)
            {
                titleText.text = winText;
                imageContainer.color = winColor;
            }
            else
            {
                titleText.text = loseText;
                imageContainer.color = loseColor;
            }

            winnerText.text = winnerText.text.Replace("%P", uiManager.GetPlayerUserName(winningPlayer.Id));
        }
    }
}