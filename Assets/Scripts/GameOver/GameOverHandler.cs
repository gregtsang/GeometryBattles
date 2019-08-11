using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;

namespace GeometryBattles.Networking
{
    public class GameOverHandler : MonoBehaviour
    {
        [SerializeField] int timeBeforeReset = 10;
        [SerializeField] TextMeshProUGUI countdownText = null;
        
        // Start is called before the first frame update
        void Start()
        {
            EventManager.onGameOver += GameOver;
        }

        private void GameOver(GameObject winner)
        {
            StartCoroutine(runCountdown());
        }

        IEnumerator runCountdown()
        {
            int time = timeBeforeReset;
            while (time > 0)
            {
                countdownText.text = time.ToString();
                time--;
                yield return new WaitForSeconds(1);
            }
            ResetGame();
        }

        private void ResetGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(0);
            }
        }
    }
}