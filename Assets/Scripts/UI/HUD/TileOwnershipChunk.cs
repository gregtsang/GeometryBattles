using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace GeometryBattles.UI
{
    public class TileOwnershipChunk : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        
        [SerializeField] TextMeshProUGUI percentText = null;
        [SerializeField] [Range(0, 1)] float minPercentDisplay = 0.1f;

        TileOwnershipDisplay tileOwnershipDisplay;
        int playerID = 0;

        public TileOwnershipDisplay TileOwnershipDisplay { get => tileOwnershipDisplay; set => tileOwnershipDisplay = value; }
        public int PlayerID { get => playerID; set => playerID = value; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            percentText.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            percentText.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            float percentOwned = tileOwnershipDisplay.GetPercentOwned(playerID);
            if (percentOwned > minPercentDisplay)
            {
                percentText.text = tileOwnershipDisplay.GetPercentOwned(playerID).ToString("P0");
            }
            else
            {
                percentText.text = "";
            }
        }
    }
}