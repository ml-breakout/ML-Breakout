using Unity.Collections;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // *****************************
    // * PUBLIC VARIABLES -> START *
    // *****************************

    [Header("Gameplay")]
    public float initialSpeed = 5f;
    public float ballX;
    public float ballY;    
    public bool discretePaddleAngle = false;

    // ***************************
    // * PUBLIC VARIABLES -> END *
    // ***************************

    // ******************************
    // * PRIVATE VARIABLES -> START *
    // ******************************

    private Rigidbody2D rb;
    private GameObject parent;   
    private bool TopWallCollsion = false; 

    [Header("Audio")]
    [SerializeField] private AudioClip paddleCollisionClip;
    [SerializeField] private AudioClip brickCollisionClip;
    [SerializeField] private AudioClip wallCollisionClip;  

    private Vector2 currentDirection;
    private float currentSpeed;


    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = transform.parent.gameObject;
        currentSpeed = (parent != null) ? parent.GetComponent<GameManager>().currentBallSpeed : currentSpeed ;
        rb = GetComponent<Rigidbody2D>();
        
        float x = UnityEngine.Random.Range(-1.0f,1.0f);
        float y = UnityEngine.Random.Range(0.5f,1.0f);

        currentDirection = new Vector2(x,y).normalized;
        rb.linearVelocity = currentDirection * currentSpeed;

    }

    void FixedUpdate()
    {
        // Keep the ball at constant speed
        rb.linearVelocity = currentDirection * initialSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        currentDirection = rb.linearVelocity.normalized;
        // used for audio clips and ball paddle collision
        if (collision.gameObject.CompareTag("Side Wall"))
        {
            SoundFXManager.instance.PlaySoundFXClip(wallCollisionClip, transform, 1f);
        }
        if (collision.gameObject.CompareTag("Brick"))
        {
            SoundFXManager.instance.PlaySoundFXClip(brickCollisionClip, transform, 1f);
        }
        if (collision.gameObject.CompareTag("Top Wall"))
        {
            SoundFXManager.instance.PlaySoundFXClip(wallCollisionClip, transform, 1f);
            // reducing paddle size by half *game feature*
            if(!TopWallCollsion){
                TopWallCollsion = true;
                if(parent != null){ 
                    // TODO re-enable this feature. We're disabling it for now because it's causing issues in training.
                    //   See issue #96 for details.
                    // 4/23/2025
                    // Re-Enabling dynamic paddle size reduction
                    // Re-comment the below line to disable
                    parent.GetComponent<GameManager>().UpdatePaddleSize();
                }
            }
        }
        if (collision.gameObject.CompareTag("Paddle"))
        {
            SoundFXManager.instance.PlaySoundFXClip(paddleCollisionClip, transform, 1f);
            // Calculate how far from the center of the paddle the ball hit
            float hitPoint = (transform.position.x - collision.transform.position.x) / collision.collider.bounds.size.x;

            // Calculate new angle based on hit point
            float bounceAngle = hitPoint * 60f; // 60 degrees max angle
            // discrete vs continous angle option
            bounceAngle = (discretePaddleAngle) ?  Mathf.Round(bounceAngle / 10f) * 10f : bounceAngle;

            // Calculate new direction
            float angleInRadians = bounceAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
            // Apply the new velocity
            currentDirection = direction;
            rb.linearVelocity = direction * initialSpeed;

            // Increment bounces
            parent.GetComponent<GameManager>().IncrementBounces();   

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "DeathZone")
        {
            if (parent != null){

                if (parent.GetComponent<GameManager>().IsTrainingMode)
                {
                    return; // let the Agent handle the ball in training mode
                }

                if (parent.GetComponent<GameManager>().LoseALife() <= 0)
                {   
                    rb.linearVelocity = new Vector2(0f, 0f);
                    currentDirection = new Vector2(0f, 0f);
                    Vector2 pos = parent.GetComponent<GameManager>().GetBallStartingPosition();
                    this.transform.position = pos;

                }else{
                    parent.GetComponent<GameManager>().ResetBall();
                }
            }
        }
    }   

    public void IncreaseBallSpeed(float amount){
        currentSpeed += amount;
        parent.GetComponent<GameManager>().currentBallSpeed += amount;
    }
}

