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

        [SerializeField] [Range(0, 2)] float textColorThreshold = 0.5f;
        [SerializeField] [Range(0, 1)] float textColorLerpAmount = 0.8f;

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

        public void UpdateTextColor()
        {
            percentText.color = Color.black;
            // Color color = MyColor();
            // Color lerpToColor;
            // float H, S, V;
            // Color.RGBToHSV(color, out H, out S, out V);
            // if (V < textColorThreshold)
            // {
            //     lerpToColor = Color.white;
            // }
            // else
            // {
            //     lerpToColor = Color.black;
            // }

            // percentText.color = Color.Lerp(color, lerpToColor, textColorLerpAmount);
        }

        Color MyColor()
        {
            return GetComponent<Image>().color;
        }
    }
}