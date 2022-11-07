using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public Vector3 velocity;
    public float bulletSpeed = 5f;
    public float actualSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetDirection(Vector3 direction) {
        velocity = direction.normalized * bulletSpeed;
        actualSpeed = velocity.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;

        if (transform.position.x < -10 || transform.position.x > 10 || transform.position.y < -5 || transform.position.y > 5) {
            Destroy(gameObject); // Clean up out-of-bounds bullets. Bullets don't wrap.
        }
    }
}
