using Unity.MLAgents.Integrations.Match3;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // *****************************
    // * PUBLIC VARIABLES -> START *
    // *****************************

    public float initialSpeed = 5f;
    public float ballX;
    public float ballY;    

    // ***************************
    // * PUBLIC VARIABLES -> END *
    // ***************************

    // ******************************
    // * PRIVATE VARIABLES -> START *
    // ******************************

    private Rigidbody2D rb;
    private GameObject parent;   
    private bool TopWallCollsion = false; 
    [SerializeField] private AudioClip paddleCollisionClip;
    [SerializeField] private AudioClip brickCollisionClip;
    [SerializeField] private AudioClip wallCollisionClip;  


    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = transform.parent.gameObject;
        rb = GetComponent<Rigidbody2D>();
        
        float x = UnityEngine.Random.Range(-1.0f,1.0f);
        float y = UnityEngine.Random.Range(0.5f,1.0f);

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
        }
        if (collision.gameObject.CompareTag("Paddle"))
        {
            SoundFXManager.instance.PlaySoundFXClip(paddleCollisionClip, transform, 1f);
            // Calculate how far from the center of the paddle the ball hit
            if(transform.position.y > collision.transform.position.y){
                float hitPoint = (transform.position.x - collision.transform.position.x) / collision.collider.bounds.size.x;

                // Calculate new angle based on hit point
                float bounceAngle = hitPoint * 60f; // 60 degrees max angle

                // Calculate new direction
                float angleInRadians = bounceAngle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
                // Apply the new velocity
                rb.linearVelocity = direction * initialSpeed;

                // Increment bounces
                parent.GetComponent<GameManager>().IncrementBounces();
            }
            
        }
        // reducing paddle size by half *game feature*
        if (collision.gameObject.CompareTag("Top Wall") && !TopWallCollsion)
        {
            TopWallCollsion = true;
            if(parent != null){ 
                // TODO re-enable this feature. We're disabling it for now because it's causing issues in training.
                //   See issue #96 for details.
                // parent.GetComponent<GameManager>().UpdatePaddleSize();
            }
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
    }
}
