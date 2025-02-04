using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;

// This class is similar to the PaddleController class, but contains modifications
// that allow the paddle to be controlled by an ML Agent.
public class PaddleAgentController : Agent
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

    [SerializeField]
    private float movementSpeed = 300f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private GameObject ball;
    private List<int> currentBricksAlive = new List<int>();
    private int prevScore = 0;
    private int newScore = 0;
    private float ball_min_x = -3.140096f;
    private float ball_max_x = 5.674098f;
    private float ball_min_y = -4.282911f;
    private float ball_max_y = 4.388294f;
    private float paddle_min_x = -2.667672f;
    private float paddle_max_x = 5.202328f;
    private float ball_velocity_min_x = -5f;
    private float ball_velocity_max_x = 5f;
    private float ball_velocity_min_y = -5f;
    private float ball_velocity_max_y = 5f;

    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************
    
    Rigidbody2D m_BallRb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Initialize()
    {

    }

    // Normalizes a value between 0 and 1
    private float NormalizeNonnegative(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    // Normalizes a value between -1 and 1
    private float Normalize(float value, float min, float max)
    {
        return 2 * (value - min) / (max - min) - 1;
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
        prevScore = 0;
    }

    // Collects observations from the environment to be used by the agent.
    // This is the input to the agent.
    public override void CollectObservations(VectorSensor sensor)
    {
        ball = gameManager.GetBall();
        m_BallRb = ball.GetComponent<Rigidbody2D>();

        // Ball position
        float ball_x = NormalizeNonnegative(ball.transform.localPosition.x, ball_min_x, ball_max_x);
        float ball_y = NormalizeNonnegative(ball.transform.localPosition.y, ball_min_y, ball_max_y);
        sensor.AddObservation(ball_x);
        sensor.AddObservation(ball_y);

        // Paddle position
        float paddle_x = NormalizeNonnegative(gameObject.transform.localPosition.x, paddle_min_x, paddle_max_x);
        sensor.AddObservation(paddle_x);

        float ball_velocity_x = Normalize(m_BallRb.linearVelocity.x, ball_velocity_min_x, ball_velocity_max_x);
        float ball_velocity_y = Normalize(m_BallRb.linearVelocity.y, ball_velocity_min_y, ball_velocity_max_y);
        sensor.AddObservation(ball_velocity_x);
        sensor.AddObservation(ball_velocity_y);

        // TODO possibly set to false during early stages of academy training
        bool watch_bricks = true;
        // bricks 
        currentBricksAlive = gameManager.GetBricksAlive();
        for (int i = 0; i < currentBricksAlive.Count; i++)
        {
            if (watch_bricks)
            {
                sensor.AddObservation(currentBricksAlive[i]);
            }
            else
            {
                sensor.AddObservation(0);
            }
        }
    }

    // Executes the actions requested by the agent and grants rewards based on the current game state.
    // This is the output of the agent.
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Move the paddle
        float move = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        rb.linearVelocity = new Vector2(move, 0) * movementSpeed;

        // end the eposide if the ball passes the paddle
        if (ball.transform.localPosition.y < gameObject.transform.localPosition.y)
        {
            // penalize the agent for missing the ball
            SetReward(-1f);
            if (gameManager.IsTrainingMode)
            {
                EndEpisode();
                return;
            }
        }

        // reward the agent for keeping the ball in play
        float curriculumStage = Academy.Instance.EnvironmentParameters.GetWithDefault("stage", 0);
        if (curriculumStage == 1.0f)
        {
            SetReward(0.1f);  // Small reward for not losing
        }

        // reward the agent for increasing the score
        if (curriculumStage == 2.0f)
        {
            newScore = gameManager.GetScore();
            int difference = newScore - prevScore;
            prevScore = newScore;
            switch (difference)
            {
                case 0:
                    SetReward(0.0f);
                    break;
                case 1:
                    SetReward(0.01f);
                    break;
                case 3:
                    SetReward(0.03f);
                    break;
                case 5:
                    SetReward(0.05f);
                    break;
                case 7:
                    SetReward(0.07f);
                    break;
                default:
                    //Combo of blocks
                    SetReward(0.04f);
                    break;
            }
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        float curriculumStage = Academy.Instance.EnvironmentParameters.GetWithDefault("stage", 0);
        if (collision.gameObject.CompareTag("Ball") && (curriculumStage == 0.0f))
        {
            SetReward(0.1f);  // Small reward for each hit
        }
    }
}
