using UnityEngine;

public class ShipScript : MonoBehaviour
{
    public GameObject bullet;
    public float speed;
    public Vector3 velocity;
    public AudioClip deathClip;
    public AudioClip gameOverClip;
    public AudioClip shootClip;

    public float acceleration = 5f;
    public float friction = 1.5f;
    public float topSpeed = 8f;
    public float minFireGap = 0.5f;
    float timeSinceFire = 0;
    // Start is called before the first frame update
    void Start()
    {
        timeSinceFire = minFireGap;
    }

    private float Wrap(float low, float high, float cur) {
        float dist = high - low;
        while (cur < low) cur += dist;
        while (cur > high) cur -= dist;
        return cur;
    }

    // Update is called once per frame
    void Update()
    {
        // The ship faces toward the mouse, and will acceleration the direction it's facing.
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = transform.position.z;
        Vector3 direction = (mouse - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        // Accelerate in _direction_ if W is pressed. Brake against velocity if nothing is pressed.
        bool accelerate = Input.GetKey("w") || Input.GetKey("up");
        if (accelerate) {
            float deltaV = acceleration*Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity + deltaV * direction, topSpeed);
        } else {
            float deltaV = friction*Time.deltaTime;
            if (velocity.magnitude < deltaV) velocity = velocity * 0f;
            else velocity -= deltaV * velocity.normalized;
        }
        transform.position += velocity * Time.deltaTime;
        speed = velocity.magnitude;

        // The ship wraps around the edges of the screen
        transform.position = new Vector3(
            Wrap(-10, 10, transform.position.x),
            Wrap(-5, 5, transform.position.y),
            transform.position.z);

        // Fire bullets on mouse click
        timeSinceFire = timeSinceFire + Time.deltaTime;
        if (Input.GetMouseButton(0) && timeSinceFire >= minFireGap) {
            timeSinceFire = 0;
            AudioSource.PlayClipAtPoint(shootClip, transform.position);
            GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation * bullet.transform.rotation);
            newBullet.GetComponent<LaserScript>().SetDirection(direction);
        }
    }
    void OnTriggerEnter2D(Collider2D other) {;
        if (other.gameObject.CompareTag("rock")) {
            AudioSource.PlayClipAtPoint(deathClip, transform.position);
            AudioSource.PlayClipAtPoint(gameOverClip, transform.position);
            Destroy(gameObject);
        }
    }

}
