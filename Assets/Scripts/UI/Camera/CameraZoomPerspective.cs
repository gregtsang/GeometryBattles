using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.UI
{
    public class CameraZoomPerspective : MonoBehaviour
    {
        [SerializeField] float zoomFactor = 1.1f;
        [SerializeField] float minZoom = 20f;
        [SerializeField] float maxZoom = 500f;
        [SerializeField] float minRotation = 13f;
        [SerializeField] float maxRotation = 90f;

        Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
            CalcZoomAndRotation();
        }

        private void OnGUI()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                CalcZoomAndRotation();
            }
        }

        void CalcZoomAndRotation()
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ViewportPointToRay( new Vector3(0.5f,0.5f,0)), out hit))
            {
                float curDistance = Vector3.Distance(cam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0)), hit.point);
                float zoomAmount = -1 * Input.mouseScrollDelta.y * zoomFactor;
                float newDistance = Mathf.Clamp(curDistance + zoomAmount, minZoom, maxZoom);

                cam.transform.position = Vector3.MoveTowards(cam.transform.position, hit.point, curDistance - newDistance);
                
                float newRotation = minRotation + (newDistance - minZoom) * ((maxRotation - minRotation) / (maxZoom - minZoom));
                cam.transform.RotateAround(hit.point, Vector3.left, transform.rotation.eulerAngles.x - newRotation);
            }
        }
    }
}