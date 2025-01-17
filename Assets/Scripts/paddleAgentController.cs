using UnityEngine;
using UnityEngine.InputSystem;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

// This class is similar to the PaddleController class, but contains modifications
// that allow the paddle to be controlled by an ML Agent.
public class PaddleAgentController : Agent
{
    [SerializeField]
    private float movementSpeed = 300f;
    private Rigidbody2D rb;

    private Vector2 movementDirection;
    public KeyCode left;
    public KeyCode right;

    private GameObject ball;
    public GameManager gameManager;
    Rigidbody2D m_BallRb;

    public Vector2 gameCenterPosition;
    public float gameWidth;
    public float paddleSize;

    public override void Initialize()
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (Input.GetKey(left))
        {
            movementDirection = new Vector2(-1, 0);
        }
        else if (Input.GetKey(right))
        {
            movementDirection = new Vector2(1, 0);
        }
        else
        {
            movementDirection = new Vector2(0, 0);
        }
        rb.linearVelocity = movementDirection * movementSpeed * Time.deltaTime;
    }

    // Specifies what should happen when a a training episode begins.
    // Training is comprised of running many episodes where the agent is contronted with different
    // scenarios and learns to make decisions based on the observations it makes.
    // The goal of this method is to initialize a wide variety of scenarios for the agent to
    // learn from.
    public override void OnEpisodeBegin()
    {
        gameManager.InitializeGame();

        ball = gameManager.GetBall();
        m_BallRb = ball.GetComponent<Rigidbody2D>();

        // randomize the ball position
        // ball.transform.position = new Vector2(Random.Range(0f, 8f), Random.Range(-3f, -1f));

        // randomize the ball direction
        //float x = Random.Range(-1f, 1.0f);
        // float y = Random.Range(0.5f, 1.0f);
        Vector2 direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f));
        float ballspeed = 5f;
        m_BallRb.linearVelocity = direction.normalized * ballspeed;

        // randomize the paddle position (within normal bounds)
        if (gameManager.IsTrainingMode)
        {
            gameObject.transform.position = new Vector2(
                Random.Range(
                  gameManager.gameCenter.x - gameWidth / 2, 
                  gameManager.gameCenter.x + gameWidth / 2), 
                gameManager.gameCenter.y
            );
        }

        // TODO: reset the blocks
    }

    // Collects observations from the environment to be used by the agent.
    // This is the input to the agent.
    public override void CollectObservations(VectorSensor sensor)
    {
        // Ball position
        sensor.AddObservation(ball.transform.position.x);
        sensor.AddObservation(ball.transform.position.y);

        // Paddle position
        sensor.AddObservation(gameObject.transform.position.x);
        sensor.AddObservation(gameObject.transform.position.y);

        // Ball velocity
        // TODO (is it ok to directly add a vec3, or do I need to deconstruct it?)
        sensor.AddObservation(m_BallRb.linearVelocity.x);
        sensor.AddObservation(m_BallRb.linearVelocity.y);
    }

    // Executes the actions requested by the agent and grants rewards based on the current game state.
    // This is the output of the agent.
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Move the paddle
        float move = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        rb.linearVelocity = new Vector2(move, 0) * movementSpeed;

        // end the eposide if the ball passes the paddle
        if (ball.transform.position.y < gameObject.transform.position.y)
        {
            SetReward(-1f);
            if (gameManager.IsTrainingMode)
            {
                EndEpisode();
            }
        }
        else
        {
            // reward the agent for keeping the ball in play
            SetReward(0.01f);
        }
    }

    // This method allows us to manually control the agent's game object.
    // Mainly used to test that the agents actions are wired properly.
    // To use this method, the agent must be in "Heuristic" mode--set this in the Unity Inspector.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(left))
        {
            continuousActionsOut[0] = -1f;
        }
        else if (Input.GetKey(right))
        {
            continuousActionsOut[0] = 1f;
        }

    }

    public void updatePaddleSize()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x / 2, this.transform.localScale.y, this.transform.localScale.z);
        paddleSize /= 2;
    }
}
