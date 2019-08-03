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
        //cached References
        UIManager uiManager;
        Board board;

        RectTransform rectTransform;


        List<RectTransform> rects = new List<RectTransform>();

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
                GameObject newImage = CreateSubImage("Player " + i, board.boardState.GetPlayer(i).GetColor());
                rects.Add(newImage.GetComponent<RectTransform>());
            }
        } 

        private void CreateBackgroundImage()
        {
            CreateSubImage("Background", Color.white);
        }

        private GameObject CreateSubImage(string name, Color color)
        {
            GameObject newImage = new GameObject(name);
            newImage.AddComponent<Image>().color = color;
            newImage.transform.SetParent(transform);

            RectTransform newRectTransform = newImage.GetComponent<RectTransform>();
            newRectTransform.offsetMin = new Vector2(0, 0);
            newRectTransform.offsetMax = new Vector2(0, 0);
            newRectTransform.anchorMin = new Vector2(0, 0);
            newRectTransform.anchorMax = new Vector2(1, 1);
            return newImage;
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
                widthFraction = (float) GetOwnershipForPlayer(i) / (float) board.boardState.GridCount;
                SetRectWidth(rects[i], widthFraction, startFraction);
                startFraction += widthFraction;
            }
        }

        private int GetOwnershipForPlayer(int pid)
        {
            return board.boardState.GetPlayer(pid).GetNumTiles();
        }
    }
}