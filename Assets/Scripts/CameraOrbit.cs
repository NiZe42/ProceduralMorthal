using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float rotationSpeed;
    private float distance;
    private float pitch;

    private float yaw;

    private void Start()
    {
        Vector3 dir = transform.position - Vector3.zero;
        distance = dir.magnitude;

        yaw   = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        pitch = Mathf.Asin(dir.y / distance) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        HandleRotation();
        UpdateCameraPosition();
    }

    private void HandleRotation()
    {
        yaw += rotationSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -80f, 80f);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rot      = Quaternion.Euler(pitch, yaw, 0);
        Vector3    position = rot * (Vector3.back * distance);

        transform.position = position;
        transform.LookAt(Vector3.zero);
    }
}