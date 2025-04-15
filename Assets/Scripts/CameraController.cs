using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed = 100f;

    // Small offset to position the camera just behind or inside the player
    public Vector3 cameraOffset = new Vector3(0f, -0.6f, 0f); // Y = eye level, Z = slightly behind
    private float currentAngle = 0f;

    void LateUpdate()
    {
        if (player == null) return;

        // Get input for rotation
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentAngle -= rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            currentAngle += rotationSpeed * Time.deltaTime;
        }

        // Get the desired position by rotating around the player
        Quaternion rotation = Quaternion.Euler(0f, currentAngle, 0f);
        Vector3 rotatedOffset = rotation * cameraOffset;
        Vector3 targetPos = player.transform.position + rotatedOffset;
        targetPos.y = targetPos.y - 1;

        // Set camera position to inside or just behind the player
        transform.position = targetPos;

        // Always look in the same direction the camera is facing
        transform.rotation = rotation;

    }
}