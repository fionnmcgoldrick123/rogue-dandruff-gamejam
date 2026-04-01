using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    void Update()
    {
        GetMousePosition();
        HandleShootingInput();
    }

    private void GetMousePosition()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void HandleShootingInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Shoot!");
        }
    }
}
