using UnityEngine;

public class UICameraFollow : MonoBehaviour
{
    public Transform targetCamera;

    void LateUpdate()
    {
        if (targetCamera != null)
        {
            transform.position = targetCamera.position;
            transform.rotation = targetCamera.rotation;
        }
    }
}
