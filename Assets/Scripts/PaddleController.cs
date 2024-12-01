using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 300f;
    private Rigidbody2D rb;

    private Vector2 movementDirection;
    public KeyCode left;
    public KeyCode right;

    // preventing going into other sides
    public Vector2 startingPosition;
    public float gameWidth;
    public float paddleSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate(){
        if(Input.GetKey(left) && (startingPosition.x - gameWidth/2 < transform.position.x - paddleSize/2))
        {
            movementDirection = new Vector2(-1,0);
        }
        else if(Input.GetKey(right) && ( startingPosition.x + gameWidth/2 > transform.position.x + paddleSize/2))
        {
            movementDirection = new Vector2(1,0);
        }else
        {
            movementDirection = new Vector2(0,0);
        }
        rb.linearVelocity = movementDirection * movementSpeed * Time.deltaTime;
    }

    public void updatePaddleSize(){
        this.transform.localScale = new Vector3 (this.transform.localScale.x/2,this.transform.localScale.y,this.transform.localScale.z);
        paddleSize /= 2;
    }
}
