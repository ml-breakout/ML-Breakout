using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using System.Linq;
using System;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Integrations.Match3;

// This class is similar to the PaddleController class, but contains modifications
// that allow the paddle to be controlled by an ML Agent.
public class PaddleAgentControllerCNN : PaddleController
{

    // *****************************
    // * PUBLIC VARIABLES -> START *
    // *****************************

    // public KeyCode left;
    // public KeyCode right;
    // public GameManager gameManager;
    // public Vector2 gameCenterPosition;
    // public float gameWidth;
    // public float paddleSize;

    // ***************************
    // * PUBLIC VARIABLES -> END *
    // ***************************


    // ******************************
    // * PRIVATE VARIABLES -> START *
    // ******************************

    [SerializeField]
    private float movementSpeed = 300f;
    private Rigidbody2D rb;
    private BehaviorParameters BP;
    private Vector2 movementDirection;
    private GameObject ball;
    private float ball_min_x = -4.5f;
    private float ball_max_x = 4.5f;
    private float ball_min_y = -4.75f;
    private float ball_max_y = 4.75f;
    private bool humanControl = false;

    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************

    Rigidbody2D m_BallRb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        BP = GetComponent<BehaviorParameters>();
        if (MainManager.Instance != null)
        {
            Debug.Log("Difficulty: " + MainManager.Instance.getDifficulty());
            BP.Model = MainManager.Instance.getDifficulty();
        }
        GameManager parent = GetComponentInParent<GameManager>();
        if(parent != null){
            movementSpeed = parent.paddleSpeed;
        }

        var behavior = GetComponent<BehaviorParameters>();
        var model = behavior.Model;
        Debug.Log("Inference device: " + behavior.InferenceDevice);
    }

    public override void Initialize()
    {

    }

    void Update()
    {
        if (!humanControl)
        {
            return;
        }
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
        rb.linearVelocity = movementDirection * movementSpeed;
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
        transform.localPosition = new Vector2(0f,-3.75f);

    }

    // Executes the actions requested by the agent and grants rewards based on the current game state.
    // This is the output of the agent.
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Move the paddle
        float move = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        rb.linearVelocity = new Vector2(move, 0) * movementSpeed;

        // Conditions for ending the episode
        if (gameManager.IsTrainingMode)
        {
            var score = gameManager.GetScore();
            // end the episode if the agent wins (and there's no more bricks to break)
            if (score == 448) {
                EndEpisode();
                return;
            }

            // end the episode if the ball passes the paddle
            if (ball.transform.localPosition.y < gameObject.transform.localPosition.y)
            {
                // Penalize the agent for missing the ball
                AddReward(-1.0f);
                EndEpisode();
                return;
            }
            Vector2 direction = m_BallRb.linearVelocity.normalized;
            if ((direction.x == 1) ||( direction.x == -1))
            {
                EndEpisode();
                return;
            }

            // end the episode if the ball goes out of bounds
            float epsilon = 1f;
            if (ball.transform.localPosition.x < (ball_min_x - epsilon) || ball.transform.localPosition.x > (ball_max_x + epsilon) ||
                ball.transform.localPosition.y < (ball_min_y - epsilon) || ball.transform.localPosition.y > (ball_max_y + epsilon))
            {
                EndEpisode();
                return;
            }
        }
    }

    /// <summary>
    /// Enables human control mode for testing the agent's behavior.
    /// </summary>
    /// <param name="actionsOut">The action buffer to be filled with human input</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        humanControl = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            AddReward(0.1f);  // Small reward for each hit
        }
    }

    public void brickScoreUpdate()
    {
        AddReward(0.1f);
    }
}
