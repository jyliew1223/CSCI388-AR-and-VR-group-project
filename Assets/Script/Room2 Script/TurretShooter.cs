using UnityEngine;

public class TurretShooter : MonoBehaviour
{
    [Header("Firing Setup")]
    public GameObject bulletPrefab;
    public Transform muzzleTransform;
    public float bulletSpeed = 50f;


    public void FireBullet()
    {
        if (bulletPrefab != null && muzzleTransform != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, muzzleTransform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = muzzleTransform.forward * bulletSpeed;
                Debug.Log("Bullet fired!");
            }
            else
            {
                Debug.LogWarning("No Rigidbody found on bullet prefab!");
            }
        }
        else
        {
            Debug.LogWarning("Missing bulletPrefab or muzzleTransform reference!");
        }
    }
}
