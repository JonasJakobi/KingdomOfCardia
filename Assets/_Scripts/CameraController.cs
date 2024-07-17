using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 1.0f;    // Speed of zooming
    public float panSpeed = 0.5f;     // Speed of panning
    public float minZoom = 5.0f;      // Minimum zoom level
    public float maxZoom = 20.0f;     // Maximum zoom level

    private Vector3 dragOrigin;       // Where the mouse drag started
    private bool isPanning;           // Flag to check if we are currently panning
    //Start values
    private float startZoom;
    private Vector3 startPosition;
    private void Start()
    {
        startZoom = Camera.main.orthographicSize;
        startPosition = Camera.main.transform.position;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.orthographicSize = startZoom;
            Camera.main.transform.position = startPosition;
        }
        HandleZoom();
        HandlePan();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float newSize = Camera.main.orthographicSize - scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    void HandlePan()
    {
        if (Input.GetMouseButton(1))  // Right mouse button pressed
        {
            float moveX = -Input.GetAxis("Mouse X") * panSpeed * Camera.main.orthographicSize / 10;
            float moveY = -Input.GetAxis("Mouse Y") * panSpeed * Camera.main.orthographicSize / 10;
            Camera.main.transform.Translate(new Vector3(moveX, moveY, 0), Space.World);
        }
    }
}


