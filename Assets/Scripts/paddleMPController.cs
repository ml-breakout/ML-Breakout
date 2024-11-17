using UnityEngine;
using UnityEngine.InputSystem;

public class paddleMPController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 300f;
    private Rigidbody2D rb;

    private Vector2 movementDirection;
    public KeyCode left;
    public KeyCode right;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // public void onMove(InputAction.CallbackContext context){
    //     movementDirection = context.ReadValue<Vector2>();
    // }
    // Update is called once per frame
    void Update()
    {
        // movementDirection = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
    }

    void FixedUpdate(){
        if(Input.GetKey(left)){
            movementDirection = new Vector2(-1,0);
        }else if(Input.GetKey(right)){
            movementDirection = new Vector2(1,0);
        }else{
            movementDirection = new Vector2(0,0);
        }
        rb.linearVelocity = movementDirection * movementSpeed * Time.deltaTime;
    }
}
