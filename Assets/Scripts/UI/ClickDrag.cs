using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.UI
{
    public class ClickDrag : MonoBehaviour
    {
        Vector3 prevMousePos;
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
                prevMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(mouseButton))
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - prevMousePos);
                transform.Translate(-1* pos.x * dragSpeed * cam.orthographicSize, -1 * pos.y * dragSpeed * cam.orthographicSize, 0);
                prevMousePos = Input.mousePosition;
            }
        }
    }
}