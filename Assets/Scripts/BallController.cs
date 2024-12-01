using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Video;

public class BallController : MonoBehaviour
{
    // Adding this line to test pull requests - Tyler 20241114
    // You can set this in the Unity Inspector
    public float initialSpeed = 5f;
    public float ballX;
    public float ballY;
    private Rigidbody2D rb;
    private GameObject parent;   

    private bool TopWallCollsion = false;
    private string paddleCollisionSoundName = "361266__japanyoshithegamer__8-bit-soft-beep-impact";
    private string brickCollisionSoundName = "752671__etheraudio__sqr-blip-2";
    private string wallCollisionSoundName = "752736__etheraudio__square-blip-non-fade";
    private AudioSource paddleCollisionSoundSource;
    private AudioSource brickCollisionSoundSource;
    private AudioSource wallCollisionSoundSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = transform.parent.gameObject;
        rb = GetComponent<Rigidbody2D>();
        
        float x = UnityEngine.Random.Range(-1.0f,1.0f);
        float y = UnityEngine.Random.Range(0.5f,1.0f);

        Vector2 direction = new Vector2(x,y);
        rb.linearVelocity = direction.normalized * initialSpeed;

        //audio
        initAudio();
    }

    public void initAudio()
    {
        AudioSource[] audio_options = GetComponents<AudioSource>();
        //GetComponent<AudioSource>().Play();
        foreach (AudioSource source in audio_options)
        {
            if (string.Equals(source.clip.name, paddleCollisionSoundName) is true)
            {
                paddleCollisionSoundSource = source;
            }
            else if(string.Equals(source.clip.name, wallCollisionSoundName) is true)
            {
                wallCollisionSoundSource = source;
            }
            else if(string.Equals(source.clip.name, brickCollisionSoundName) is true)
            {
                brickCollisionSoundSource = source;
            }
        }
    }

    void Update()
    {
        // Keep the ball at constant speed
        rb.linearVelocity = rb.linearVelocity.normalized * initialSpeed;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Side Wall"))
        {
            wallCollisionSoundSource.Play();
        }
        if (collision.gameObject.CompareTag("Brick"))
        {
            brickCollisionSoundSource.Play();
        }
        if (collision.gameObject.CompareTag("Top Wall"))
        {
            wallCollisionSoundSource.Play();
        }
        if (collision.gameObject.CompareTag("Paddle"))
        {
            paddleCollisionSoundSource.Play();
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
        if (collision.gameObject.CompareTag("Top Wall") && !TopWallCollsion)
        {
            TopWallCollsion = true;
            if(parent != null){
                parent.GetComponent<GameManager>().UpdatePaddleSize();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "DeathZone")
        {
            if (parent != null){
                if (parent.GetComponent<GameManager>().trainingMode)
                {
                    return; // let the Agent handle the ball in training mode
                }
                
                Vector2 pos = parent.GetComponent<GameManager>().GetBallStartingPosition();
                rb.MovePosition(pos);
                // objects moved with the above function can still collide, could cause issues but hasn't so far
                if (parent.GetComponent<GameManager>().LoseALife() <= 0)
                {
                    rb.linearVelocity = new Vector2(0f, 0f);
                }
                else
                {
                    float x = UnityEngine.Random.Range(-1.0f,1.0f);
                    float y = UnityEngine.Random.Range(0.5f,1.0f);

                    Vector2 direction = new Vector2(x,y);
                    rb.linearVelocity = direction.normalized * initialSpeed;
                }
            }
        }
    }

    public void IncreaseBallSpeed(float amount){
        initialSpeed += amount;
        Debug.Log(initialSpeed);
    }

}
