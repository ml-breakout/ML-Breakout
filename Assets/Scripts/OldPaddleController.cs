using UnityEngine;

public class OldPaddleController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float boundaryLeft = -8f;  // Left screen boundary
    public float boundaryRight = 8f;  // Right screen boundary
    
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get horizontal input (left/right arrows or A/D keys)
        float moveInput = Input.GetAxisRaw("Horizontal");
        
        // Calculate new position
        Vector2 currentPos = transform.position;
        float newX = currentPos.x + (moveInput * moveSpeed * Time.deltaTime);
        
        // Clamp position within boundaries
        newX = Mathf.Clamp(newX, boundaryLeft, boundaryRight);
        
        // Update position
        transform.position = new Vector2(newX, currentPos.y);
    }
}