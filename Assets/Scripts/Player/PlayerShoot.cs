using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private ObjectPool bulletPool;
    [SerializeField] private MuzzleFlash muzzleFlash;

    private float nextFireTime;

    void Start()
    {
        // If gunTransform not assigned, use firePoint's parent
        if (gunTransform == null && firePoint != null)
        {
            gunTransform = firePoint.parent;
        }
    }

    void Update()
    {
        AimGunAtMouse();
        HandleShootingInput();
    }

    private void AimGunAtMouse()
    {
        if (gunTransform == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Check if mouse is on left side of player
        if (mouseWorld.x < transform.position.x)
        {
            // Rotate 180 degrees on X axis when aiming left
            gunTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.AngleAxis(180f, Vector3.right);
        }
        else
        {
            // Normal rotation when aiming right
            gunTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void HandleShootingInput()
    {
        float fireRate = stats != null ? stats.fireRate : 0.5f;
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        GameObject bulletObj = bulletPool.Get(firePoint.position, Quaternion.identity);
        bulletObj.GetComponent<Bullet>().Init(bulletPool, gunTransform.right, stats);

        // Trigger muzzle flash animation
        if (muzzleFlash != null)
        {
            muzzleFlash.PlayMuzzleFlash();
        }
        
        // Trigger camera shake
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake();
        }
        
        // Trigger crosshair shake
        Canvas crosshairCanvas = GetComponentInParent<Canvas>();
        if (crosshairCanvas != null)
        {
            Crosshair crosshair = crosshairCanvas.GetComponentInChildren<Crosshair>();
            if (crosshair != null)
            {
                crosshair.Shake();
            }
        }
    }
}
