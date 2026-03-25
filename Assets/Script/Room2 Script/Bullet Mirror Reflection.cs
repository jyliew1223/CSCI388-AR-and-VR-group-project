using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletMirrorReflect : MonoBehaviour
{
    private Rigidbody rb;
    public static GameObject activeBullet;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Destroy existing bullet if it exists, hence allowing 1 bullet
        if (activeBullet != null && activeBullet != gameObject)
        {
            Destroy(activeBullet);
        }

        activeBullet = gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Deflector"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;

            // Preserve speed while changing direction
            float speed = rb.linearVelocity.magnitude;
            Vector3 reflectedDirection = Vector3.Reflect(rb.linearVelocity.normalized, normal);
            rb.linearVelocity = reflectedDirection * speed;
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            
            Destroy(collision.gameObject); // Destroy the obstacle

            Destroy(gameObject); // Destroy the bullet
        }
    }

        private void OnDestroy()
    {
        if (activeBullet == gameObject)
        {
            activeBullet = null;
        }
    }
}
