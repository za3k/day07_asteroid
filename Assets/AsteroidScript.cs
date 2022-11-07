using Unity.Mathematics;
using UnityEngine;
using Random=UnityEngine.Random;

public class AsteroidScript : MonoBehaviour
{
    public float rotationRate; // Degrees per second
    public Vector3 velocity;
    public int size;

    public int difficulty = 2;
    public int maxDifficulty = 4;
    public AudioClip destroyedClip;
    public AudioClip winGameClip;

    private float Wrap(float low, float high, float cur) {
        float dist = high - low;
        while (cur < low) cur += dist;
        while (cur > high) cur -= dist;
        return cur;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Random rotation speed
        rotationRate = Random.Range(-10f,10f);
        
        // Random direction
        Vector3 direction;
        do {
            direction = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), 0);
        } while (direction.magnitude < 0.5 || direction.magnitude > 1);
        direction = direction.normalized;
        
        // Random speed
        float speed = Random.Range(0.5f, 5f);

        velocity = direction * speed;

        float scaleFactor = math.pow(3, -size);
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity*Time.deltaTime;
        transform.Rotate(new Vector3(0,0, rotationRate));

        // The ship wraps around the edges of the screen
        transform.position = new Vector3(
            Wrap(-10, 10, transform.position.x),
            Wrap(-5, 5, transform.position.y),
            transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("bullet")) {
            
            Destroy(obj: other.gameObject);
            AudioSource.PlayClipAtPoint(destroyedClip, transform.position);
            if (size < difficulty) {
                GameObject rock1 = Instantiate(gameObject, transform.position, transform.rotation);
                GameObject rock2 = Instantiate(gameObject, transform.position, transform.rotation);
                GameObject rock3 = Instantiate(gameObject, transform.position, transform.rotation);
                rock1.GetComponent<AsteroidScript>().size = size+1;
                rock2.GetComponent<AsteroidScript>().size = size+1;
                rock3.GetComponent<AsteroidScript>().size = size+1;
            } else {
                int totalAsteroids = GameObject.FindGameObjectsWithTag("rock").Length - 1;
                if (totalAsteroids == 0) {
                    AudioSource.PlayClipAtPoint(winGameClip, transform.position);
                    // Win and increase difficulty.
                    if (difficulty <= maxDifficulty - 1) {
                        AsteroidScript newStart = Instantiate(gameObject, transform.position, transform.rotation).GetComponent<AsteroidScript>();
                        newStart.size = 0;
                        newStart.difficulty = difficulty + 1;
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
