using UnityEngine;

public class BallVelocity : MonoBehaviour
{
    // Adding this line to test if the main branch is protected - Tyler 20241113
    // You can set this in the Unity Inspector
    public float initialSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(0f, initialSpeed);
    }

    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
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
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = direction * initialSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "DeathZone")
        {
            // Call game over
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }
}
