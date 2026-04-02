using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [Header("Mouse Following")]
    [SerializeField] private float minMouseDistance = 2f;  // Min distance to start moving camera
    [SerializeField] private float maxMouseDistance = 5f;  // Max distance camera will follow
    [SerializeField] private float cameraFollowSpeed = 5f; // How quickly camera moves towards mouse

    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        transform.position = targetPosition;
    }

    void Update()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        // Get mouse world position
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = playerTransform.position;
        
        // Calculate distance from player to mouse
        float distanceToMouse = Vector2.Distance(playerPos, mouseWorld);
        
        // Only move camera if mouse is beyond minMouseDistance
        if (distanceToMouse > minMouseDistance)
        {
            // Calculate direction from player to mouse
            Vector2 directionToMouse = (mouseWorld - playerPos).normalized;
            
            // Clamp the distance to not exceed maxMouseDistance
            float clampedDistance = Mathf.Min(distanceToMouse, maxMouseDistance);
            
            // Calculate target position with offset towards mouse
            targetPosition = new Vector3(
                playerPos.x + directionToMouse.x * clampedDistance,
                playerPos.y + directionToMouse.y * clampedDistance,
                transform.position.z
            );
        }
        else
        {
            // When mouse is close, just follow player
            targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        }
        
        // Smoothly move camera to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraFollowSpeed * Time.deltaTime);
    }
}
