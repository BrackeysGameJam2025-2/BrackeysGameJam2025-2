using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform player; // The player's transform

    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Offset from the player

    [Header("Follow Smoothness")]
    public float followSpeed = 5f; // Speed at which the camera follows the player

    private void LateUpdate()
    {
        if (player != null)
        {
            // Calculate the target position for the camera
            Vector3 targetPosition = player.position + offset;

            // Smoothly move the camera to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Ensure the camera always looks at the player
            transform.LookAt(player);
        }
    }
}
