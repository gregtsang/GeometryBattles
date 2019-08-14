using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.HexAction;
using GeometryBattles.BoardManager;
using System;

namespace GeometryBattles.UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class BombSelectionUI : MonoBehaviour
    {
        [SerializeField] SelectBombTarget bombAction = null;
        [SerializeField] float lineHeight = 5f;
        [SerializeField] int lineSegments = 100;

        bool uiEnabled = false;
        Tile sourceTile = null;
        Vector3 fromPoint;

        //Cached References
        Camera cam;
        LineRenderer lineRenderer;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            cam = Camera.main;
            
            bombAction.BombSelectionEntered += BombSelectionOn;
            bombAction.BombSelectionLeft += BombSelectionOff;

            TurnOff();
        }

        void Update()
        {
            if (uiEnabled)
            {
                Vector3 toPoint = GetPoint();
                Vector3[] points = GenerateLinePoints(fromPoint, toPoint, lineHeight, lineSegments);
                lineRenderer.positionCount = points.Length;
                lineRenderer.SetPositions(points);
            }
        }

        private void BombSelectionOff(object sender, EventArgs e)
        {
            TurnOff();
        }

        private void BombSelectionOn(object sender, BombSelectionEnteredEventArgs e)
        {
            TurnOn(e.tile);
        }

        private void TurnOff()
        {
            uiEnabled = false;
            sourceTile = null;
            lineRenderer.enabled = false;
        }

        private void TurnOn(Tile tile)
        {
            uiEnabled = true;
            sourceTile = tile;
            fromPoint = sourceTile.transform.position;
            lineRenderer.enabled = true;
        }

        private Vector3 GetPoint()
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                return hit.point;
            }
            else
            {
                return new Vector3();
            }
        }

        private Vector3[] GenerateLinePoints(Vector3 start, Vector3 end, float height, int segments)
        {
            Vector3 mid = (start + end) / 2;
            mid.y += height;
            
            var points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                float seg = ((float) i / segments);
                Vector3 newPoint = Mathf.Pow(1 - seg, 2) * start + 2 * (1 - seg) * seg * mid + Mathf.Pow(seg, 2) * end;
                points.Add(newPoint);             
            }
            
            return points.ToArray();
        }
    }
}