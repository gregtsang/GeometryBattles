using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.UI
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] float zoomSpeed = 1.0f;
        [SerializeField] float minSize = 1;
        [SerializeField] float maxSize = 20;
        
        Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void OnGUI()
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomSpeed * Input.mouseScrollDelta.y, minSize, maxSize);
        }
    }
}