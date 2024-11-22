using UnityEditor.Callbacks;
using UnityEngine;

public class BallVelocity : MonoBehaviour
{
    // Adding this line to test pull requests - Tyler 20241114
    // You can set this in the Unity Inspector
    public float initialSpeed = 5f;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        float x = Random.Range(-1.0f,1.0f);
        float y = Random.Range(0.5f,1.0f);

        Vector2 direction = new Vector2(x,y);
        rb.linearVelocity = direction.normalized * initialSpeed;
    }

    void Update()
    {
        // Keep the ball at constant speed
        rb.linearVelocity = rb.linearVelocity.normalized * initialSpeed;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            // Calculate how far from the center of the paddle the ball hit
            float hitPoint = (transform.position.x - collision.transform.position.x) / collision.collider.bounds.size.x;

            // Calculate new angle based on hit point
            float bounceAngle = hitPoint * 60f; // 60 degrees max angle

            // Calculate new direction
            float angleInRadians = bounceAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));

            // Apply the new velocity
            rb.linearVelocity = direction * initialSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "DeathZone")
        {
            if (GameManager.instance != null){
                if (GameManager.instance.trainingMode)
                {
                    return; // let the Agent handle the ball in training mode
                }
                
                GameObject ballObject = this.gameObject; 
                Vector2 pos = GameManager.instance.GetBallStartingPosition(ballObject);
                rb.MovePosition(pos);
                // objects moved with the above function can still collide, could cause issues but hasn't so far
                if (GameManager.instance.LoseALife(ballObject) <= 0)
                {
                    rb.linearVelocity = new Vector2(0f, 0f);
                }
                else
                {
                    float x = Random.Range(-1.0f,1.0f);
                    float y = Random.Range(0.5f,1.0f);

                    Vector2 direction = new Vector2(x,y);
                    rb.linearVelocity = direction.normalized * initialSpeed;
                }
            }
        }
    }
}
