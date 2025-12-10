using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target; //  object the camera will orbit around
    public float distance = 5.0f; //  distance from the target
    public float rotationSpeed = 5.0f; // rotando speed with mouse
    public float zoomSpeed = 5.0f;     // speed of zoom zoom with scroll wheel
    public float minDistance = 2.0f;   // min zoom distance
    public float maxDistance = 15.0f;  // max zoom distance
    public float smoothness = 0.1f;    

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;

    void Update()
    {
        // rotando when right mouse is held
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, -80f, 80f);
        }

        // team oomie zoomie with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // calc desired position and rotation
        desiredRotation = Quaternion.Euler(currentY, currentX, 0);
        desiredPosition = target.position - (desiredRotation * Vector3.forward * distance);
    }

    void LateUpdate()
    {
        // smoothen so it dosent jitter like my hands when im taking an above recommended dose of caffine to finish a certain game dev project
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothness);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothness);
    }
}