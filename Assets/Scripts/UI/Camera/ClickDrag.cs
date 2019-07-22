using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.UI
{
    public class ClickDrag : MonoBehaviour
    {
        Vector3 prevMousePos;
        Vector3 prevPoint;
        Camera cam;
        [SerializeField] int mouseButton = 1;
        [SerializeField] float dragSpeed = 1f;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }
        
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(mouseButton))
            {
                prevPoint = GetPoint();
            }

            if (Input.GetMouseButton(mouseButton))
            {
                Vector3 currPoint = GetPoint();
                cam.transform.Translate(prevPoint - currPoint, Space.World);
                prevPoint = GetPoint();
            }
        }

        private Vector3 GetPoint()
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //return new Vector3(hit.point.x, 0, hit.point.z);
                return hit.point;
            }
            else
            {
                return new Vector3();
            }
        }
    }
}