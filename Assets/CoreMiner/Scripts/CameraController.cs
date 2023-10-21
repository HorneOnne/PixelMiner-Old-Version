using UnityEngine;

namespace CoreMiner.WorldGen
{
    public class CameraController : MonoBehaviour
    {
        public float moveSpeed = 5f; // Camera movement speed.
        public float zoomSpeed = 5f; // Camera zoom speed.
        public float minZoom = 2f; // Minimum zoom level.
        public float maxZoom = 10f; // Maximum zoom level;
        public float edgeOffset = 10f; // Offset from the edge to trigger camera movement.

        private Camera mainCamera;

        public bool LockMove = false;

        // Smooth zoom variables
        private float targetZoom;
        public float smoothZoomTime = 0.5f;

        private void Start()
        {
            mainCamera = Camera.main; // Get the main camera in the scene.
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Y))
            {
                LockMove = !LockMove; 
            }


            if (LockMove == true) return;
            // Camera movement.
            Vector3 moveDirection = Vector3.zero;

            if (Input.mousePosition.x <= edgeOffset)
            {
                moveDirection.x = -1;
            }
            else if (Input.mousePosition.x >= Screen.width - edgeOffset)
            {
                moveDirection.x = 1;
            }

            if (Input.mousePosition.y <= edgeOffset)
            {
                moveDirection.y = -1;
            }
            else if (Input.mousePosition.y >= Screen.height - edgeOffset)
            {
                moveDirection.y = 1;
            }

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            // Camera zoom.
            float zoomInput = Input.GetAxis("Mouse ScrollWheel");
            targetZoom = Mathf.Clamp(targetZoom - zoomInput * zoomSpeed, minZoom, maxZoom);

            // Smoothly interpolate the camera size towards the target size.
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.deltaTime / smoothZoomTime);
        }
    }
}

