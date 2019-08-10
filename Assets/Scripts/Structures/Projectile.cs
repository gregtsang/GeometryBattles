using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int Q, R;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            EventManager.RaiseOnProjectileCollision(gameObject);
        }
    }
}
