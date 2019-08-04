using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeometryBattles.BoardManager;
using System;

namespace GeometryBattles.UI
{
    public class TileOwnershipDisplay : MonoBehaviour
    {
        [SerializeField] Color backgroundColor = Color.white;
        [SerializeField] GameObject playerChunkPrefab = null;

        //cached References
        UIManager uiManager;
        Board board;

        RectTransform rectTransform;

        List<RectTransform> rects = new List<RectTransform>();
        RectTransform backgroundRect;

        // Start is called before the first frame update
        void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
            board = uiManager.GetBoard();
            rectTransform = GetComponent<RectTransform>();
            CreateBackgroundImage();
            CreatePlayerImages();
        }

        void Update()
        {
            SetAllPlayerRects();
        }

        private void CreatePlayerImages()
        {
            for (int i = 0; i < board.numPlayers; i++)
            {
                GameObject newImage = CreateSubImage("Player " + i, board.boardState.GetPlayer(i).GetColor(), i);
                rects.Add(newImage.GetComponent<RectTransform>());
            }
        } 

        private void CreateBackgroundImage()
        {
            //GameObject newImage = new GameObject(name);
            //newImage.AddComponent<Image>().color = backgroundColor;
            //newImage.transform.SetParent(transform);
            //SetImageSize(newImage.GetComponent<RectTransform>());
            backgroundRect = CreateSubImage("Background", backgroundColor, -1).GetComponent<RectTransform>();
        }

        private GameObject CreateSubImage(string name, Color color, int playerID)
        {
            //GameObject newImage = new GameObject(name);
            //newImage.AddComponent<Image>().color = color;
            GameObject newImage = Instantiate(playerChunkPrefab);
            newImage.name = name;
            newImage.GetComponent<Image>().color = color;
            newImage.transform.SetParent(transform);
            newImage.GetComponent<TileOwnershipChunk>().TileOwnershipDisplay = this;
            newImage.GetComponent<TileOwnershipChunk>().PlayerID = playerID;
            newImage.GetComponent<TileOwnershipChunk>().UpdateTextColor();

            SetImageSize(newImage.GetComponent<RectTransform>());
            return newImage;
        }

        private static void SetImageSize(RectTransform newRectTransform)
        {
            newRectTransform.offsetMin = new Vector2(0, 0);
            newRectTransform.offsetMax = new Vector2(0, 0);
            newRectTransform.anchorMin = new Vector2(0, 0);
            newRectTransform.anchorMax = new Vector2(1, 1);
        }

        private void SetRectWidth(RectTransform rect, float widthFraction = 1, float startFraction = 0)
        {
            //rect.sizeDelta = new Vector2(widthFraction * rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
            rect.anchorMin = new Vector2(startFraction, 0);
            rect.anchorMax = new Vector2(startFraction + widthFraction, 1);
        }

        private void SetAllPlayerRects()
        {
            float startFraction = 0;
            float widthFraction = 0;
            for (int i = 0; i < rects.Count; i++)
            {
                widthFraction = GetPercentOwned(i); //(float) GetOwnershipForPlayer(i) / (float) board.boardState.GridCount;
                SetRectWidth(rects[i], widthFraction, startFraction);
                startFraction += widthFraction;
            }
            widthFraction = GetUnownedPercentage();
            SetRectWidth(backgroundRect, widthFraction, startFraction);
        }

        private int GetOwnershipForPlayer(int pid)
        {
            return board.boardState.GetPlayer(pid).GetNumTiles();
        }

        public float GetPercentOwned(int pid)
        {
            if (pid == -1)
            {
                return GetUnownedPercentage();
            }
            return (float) GetOwnershipForPlayer(pid) / (float) board.boardState.GridCount;
        }

        private float GetUnownedPercentage()
        {
            int ownedTiles = 0;
            for (int i = 0; i < rects.Count; i++)
            {
                ownedTiles += GetOwnershipForPlayer(rects[i].GetComponent<TileOwnershipChunk>().PlayerID);
            }
            return (float) (board.boardState.GridCount - ownedTiles) / (float) board.boardState.GridCount;
        }
    }
}