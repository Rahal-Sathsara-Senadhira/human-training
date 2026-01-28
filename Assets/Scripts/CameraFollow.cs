using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;   // Player

    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 6, -8);

    [Header("Smoothness")]
    public float smoothSpeed = 8f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            smoothSpeed * Time.deltaTime
        );

        // Always look at the player
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}
