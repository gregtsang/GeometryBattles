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
        [SerializeField] float minRotation = 13f;
        [SerializeField] float maxRotation = 90f;

        Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void OnGUI()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomSpeed * Input.mouseScrollDelta.y, minSize, maxSize);
                RaycastHit hit;
                if (Physics.Raycast(cam.ViewportPointToRay( new Vector3(0.5f,0.5f,0)), out hit))
                {
                    float newRotation = minRotation + (cam.orthographicSize - minSize) * ((maxRotation - minRotation) / (maxSize - minSize));
                    cam.transform.RotateAround(hit.point, Vector3.left, transform.rotation.eulerAngles.x - newRotation);
                }
            }
        }
    }
}