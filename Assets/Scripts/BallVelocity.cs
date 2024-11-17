using UnityEngine;

public class BallVelocity : MonoBehaviour
{
    // Adding this line to test pull requests - Tyler 20241114
    // You can set this in the Unity Inspector
    public float initialSpeed = 5f;

    // paused game on startup, noted initial ball position
    private float startingxpos = -0.7598f;
    private float startingypos = -0.4128f;

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
            // Add score when ball hits paddle
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore();
            }

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
            // Changing from instant game restart to 3 lives
            //if (GameManager.instance != null)
            //{
            //    GameManager.instance.GameOver();
            //}
            if (GameManager.instance != null)
            {
                // to reset pos and speed of ball
                // reset and freeze it if no lives remain
                Vector2 pos = new Vector2(startingxpos, startingypos);
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                // collision might need to be disabled
                rb.MovePosition(pos);
                // objects moved with the above function can still collide, could cause issues but hasn't so far
                if (GameManager.instance.LoseALife() == 0)
                {
                    rb.linearVelocity = new Vector2(0f, 0f);
                }
                else
                {
                    rb.linearVelocity = new Vector2(0f, initialSpeed);
                }
            
            }
        }
    }
}
