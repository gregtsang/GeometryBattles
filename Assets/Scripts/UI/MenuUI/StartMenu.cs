using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace GeometryBattles.MenuUI
{ 
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] GameObject startCanvas = null;
        [SerializeField] GameObject creditsCanvas = null;
        [SerializeField] GameObject gameLobbyCanvas = null;
        [SerializeField] GameObject createGameCanvas = null;
        [SerializeField] GameObject connectCanvas = null;
        [SerializeField] GameObject connectingCanvas = null;
        [SerializeField] GameObject preGameCanvas = null;

        public void QuitGame()
        {
            Application.Quit();
        }
        
        public void ShowStartMenuCanvas()
        {
            DisableAllCanvases();
            startCanvas.SetActive(true);
        }
        
        public void ShowCreditsMenuCanvas()
        {
            DisableAllCanvases();
            creditsCanvas.SetActive(true);
        }

        public void ShowGameLobbyCanvas()
        {
            DisableAllCanvases();
            gameLobbyCanvas.SetActive(true);
        }

        public void ShowCreateGameCanvas()
        {
            DisableAllCanvases();
            createGameCanvas.SetActive(true);
        }

        public void ShowConnectCanvas()
        {
            DisableAllCanvases();
            connectCanvas.SetActive(true);
        }

        public void ShowConnectingCanvas()
        {
            DisableAllCanvases();
            connectingCanvas.SetActive(true);
        }

        public void ShowPreGameCanvas()
        {
            DisableAllCanvases();
            preGameCanvas.SetActive(true);
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene(2);
        }

        private void DisableAllCanvases()
        {
            var allCanvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in allCanvases)
            {
                canvas.gameObject.SetActive(false);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            // if (PhotonNetwork.InRoom)
            // {
            //     PhotonNetwork.LeaveRoom();
            //     PhotonNetwork.LeaveLobby();
            //     PhotonNetwork.Disconnect();
            //     PhotonNetwork.JoinLobby();
            //     ShowGameLobbyCanvas();
            // }
            // else
            // {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            ShowStartMenuCanvas();
            // }
        }
    }
}