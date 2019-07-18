using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryBattles.UI
{ 
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] GameObject startCanvas = null;
        [SerializeField] GameObject creditsCanvas = null;
        
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
            ShowStartMenuCanvas();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}