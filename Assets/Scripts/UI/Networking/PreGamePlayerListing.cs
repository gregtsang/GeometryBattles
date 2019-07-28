using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GeometryBattles.MenuUI
{
    public class PreGamePlayerListing : MonoBehaviour
    {
        [SerializeField] GameObject nameText = null;
        [SerializeField] GameObject readyText = null;
        [SerializeField] private string placeholderText = "Waiting For Player...";
        
        int _actorNumber;

        public int ActorNumber { get => _actorNumber; set => _actorNumber = value; }
        
        private void Awake()
        {
            UnSetPlayer();
        }

        public void InitializeListing(int actorNumber, string playerNickName)
        {
            _actorNumber = actorNumber;
            nameText.GetComponent<TextMeshProUGUI>().text = playerNickName;
            SetReadyText(false);
        }
        
        public void UnSetPlayer()
        {
            _actorNumber = -1;
            nameText.GetComponent<TextMeshProUGUI>().text = placeholderText;
            SetReadyText(false);
        }

        public void SetReady(bool readyValue)
        {
            SetReadyText(readyValue);
        }

        public bool IsNotPlayer()
        {
            return (_actorNumber == -1);
        }

        private void SetReadyText(bool readyValue)
        {
            if (readyValue)
            {
                readyText.GetComponent<TextMeshProUGUI>().text = "READY";
            }
            else
            {
                readyText.GetComponent<TextMeshProUGUI>().text = "-----------";
            }
        }
    }
}