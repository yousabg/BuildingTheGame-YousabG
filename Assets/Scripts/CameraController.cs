using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed = 100f;

    public Vector3 cameraOffset = new Vector3(0f, -0.6f, 0f);
    private float currentAngle = 0f;

    void LateUpdate()
    {
        if (player == null) return;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentAngle -= rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            currentAngle += rotationSpeed * Time.deltaTime;
        }

        Quaternion rotation = Quaternion.Euler(0f, currentAngle, 0f);
        Vector3 rotatedOffset = rotation * cameraOffset;
        Vector3 targetPos = player.transform.position + rotatedOffset;
        targetPos.y = targetPos.y - 1;

        transform.position = targetPos;

        transform.rotation = rotation;

    }
}