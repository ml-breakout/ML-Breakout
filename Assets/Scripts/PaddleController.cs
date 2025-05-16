using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class PaddleController : Agent
{

    // *****************************
    // * PUBLIC VARIABLES -> START *
    // *****************************

    public KeyCode left;
    public KeyCode right;
    public GameManager gameManager;
    public Vector2 gameCenterPosition;
    public float gameWidth;
    public float paddleSize;

    // ***************************
    // * PUBLIC VARIABLES -> END *
    // ***************************


    // ******************************
    // * PRIVATE VARIABLES -> START *
    // ******************************
    private float original_paddle_size;
    private bool paddleSizeReduced = false;


    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************

    /// <summary>
    /// Initializes the paddle agent by setting up the rigidbody, behavior parameters, and difficulty level.
    /// </summary>
    void Start()
    {
        original_paddle_size = paddleSize;

    }

    /// <summary>
    /// Updates the paddle size by reducing it by half, but only once.
    /// </summary>
    public void updatePaddleSize()
    {
        if (!paddleSizeReduced)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x / 2, this.transform.localScale.y, this.transform.localScale.z);
            paddleSize /= 2;
        }
        paddleSizeReduced = true;
    }

    public void restorePaddleSize()
    {
        if (paddleSizeReduced)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * 2, this.transform.localScale.y, this.transform.localScale.z);
        }
        paddleSize = original_paddle_size;
        paddleSizeReduced = false;
    }
}
