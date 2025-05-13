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
    private BehaviorParameters BP;
    private Vector2 movementDirection;
    private GameObject ball;
    private int prevScore = 0;
    private int newScore = 0;
    private int score_at_last_bounce = 0;
    private float ball_min_x = -4.5f;
    private float ball_max_x = 4.5f;
    private float ball_min_y = -4.75f;
    private float ball_max_y = 4.75f;
    private float paddle_min_x = -3.5f;
    private float paddle_max_x = 3.5f;
    private float ball_velocity_min_x = -5f;
    private float ball_velocity_max_x = 5f;
    private float ball_velocity_min_y = -5f;
    private float ball_velocity_max_y = 5f;
    private float original_paddle_size;

    private bool paddleSizeReduced = false;

    private bool humanControl = false;

    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************

    Rigidbody2D m_BallRb;

    /// <summary>
    /// Initializes the paddle agent by setting up the rigidbody, behavior parameters, and difficulty level.
    /// </summary>
    void Start()
    {
        original_paddle_size = paddleSize;
        rb = GetComponent<Rigidbody2D>();
        BP = GetComponent<BehaviorParameters>();
        if (MainManager.Instance != null)
        {
            Debug.Log("Difficulty: " + MainManager.Instance.getDifficulty());
            BP.Model = MainManager.Instance.getDifficulty();
        }
    }

    /// <summary>
    /// Initializes the ML-Agent. This method is called when the agent is first created.
    /// </summary>
    public override void Initialize()
    {

    }

    /// <summary>
    /// Normalizes a value to be between 0 and 1 based on the given minimum and maximum values.
    /// </summary>
    /// <param name="value">The value to normalize</param>
    /// <param name="min">The minimum value in the range</param>
    /// <param name="max">The maximum value in the range</param>
    /// <returns>A normalized value between 0 and 1</returns>
    private float NormalizeNonnegative(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    /// <summary>
    /// Normalizes a value to be between -1 and 1 based on the given minimum and maximum values.
    /// </summary>
    /// <param name="value">The value to normalize</param>
    /// <param name="min">The minimum value in the range</param>
    /// <param name="max">The maximum value in the range</param>
    /// <returns>A normalized value between -1 and 1</returns>
    private float Normalize(float value, float min, float max)
    {
        return 2 * (value - min) / (max - min) - 1;
    }

    /// <summary>
    /// Handles human input control of the paddle when in human control mode.
    /// Moves the paddle left or right based on keyboard input.
    /// </summary>
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

    /// <summary>
    /// Initializes a new training episode. Sets up the game state, ball position, and resets scores.
    /// Also handles special initialization for quadrant training mode.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        gameManager.InitializeGame();

        // Check if we're in quadrant training mode (curriculum stage 3)
        float curriculumStage = Academy.Instance.EnvironmentParameters.GetWithDefault("stage", 0);
        if (curriculumStage == 3.0f)
        {
            gameManager.KeepOnlyOneQuadrant();
        }

        ball = gameManager.GetBall();
        m_BallRb = ball.GetComponent<Rigidbody2D>();

        prevScore = 0;
        score_at_last_bounce = 0;
        restorePaddleSize();
    }

    /// <summary>
    /// Collects and processes observations from the game environment for the ML-Agent.
    /// Includes ball position, paddle position, ball velocity, and brick status information.
    /// </summary>
    /// <param name="sensor">The vector sensor to add observations to</param>
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

        // Paddle size
        // Doesn't seem like NormalizeNonnegative needs to be used here, but could possibly cause issues
        sensor.AddObservation(paddleSize);
        // Could add some quick math to include x and y coords of edge of paddle
        float paddleLeftEdge = paddle_x - paddleSize / 2;
        float paddleRightEdge = paddle_x + paddleSize / 2;
        sensor.AddObservation(paddleLeftEdge);
        sensor.AddObservation(paddleRightEdge);

        float ball_velocity_x = Normalize(m_BallRb.linearVelocity.x, ball_velocity_min_x, ball_velocity_max_x);
        float ball_velocity_y = Normalize(m_BallRb.linearVelocity.y, ball_velocity_min_y, ball_velocity_max_y);
        sensor.AddObservation(ball_velocity_x);
        sensor.AddObservation(ball_velocity_y);

        // bricks 

        // Create 4 observations -- one for each quadrant of bricks. The value of the observation is the fraction of bricks remaining in that quadrant.
        // If we want to experiment with more or fewer partitions, we can change the values of horizontalPartitions and veriticalPartitions below.

        List<List<int>> currentBricksAlive = gameManager.GetBricksAlive();

        int totalHorizontalBricks = currentBricksAlive[0].Count;
        int totalVerticalBricks = currentBricksAlive.Count;
        int horizontalPartitions = 2;
        int veriticalPartitions = 4;

        // The first value of the tuple is the number of bricks alive in the partition, and the second value is the total number of possible bricks in the partition.
        (int, int)[,] brickCounts = new (int, int)[horizontalPartitions, veriticalPartitions];  // initialized by default (0,0) in each cell

        int horizontalPartitionSize = totalHorizontalBricks / horizontalPartitions;
        int verticalPartitionSize = totalVerticalBricks / veriticalPartitions;

        foreach (var pair in currentBricksAlive.Select((column, verticalIndex) => new { column = column, verticalIndex = verticalIndex }))
        {
            List<int> column = pair.column;
            int verticalIndex = pair.verticalIndex;

            foreach (var brick in column.Select((brick, horizontalIndex) => new { brick = brick, horizontalIndex = horizontalIndex }))
            {
                int horizontalIndex = brick.horizontalIndex;
                int brickIsAlive = brick.brick;

                int horizontalPartitionIndex = horizontalIndex / horizontalPartitionSize;
                int verticalPartitionIndex = verticalIndex / verticalPartitionSize;

                // Ensure the indices are within bounds.
                // We need to do this because if the number of bricks is not divisible by the number of partitions, 
                // there will be some bricks left over. We want to just put these in the last partition. This means
                // the last partition can contain more bricks than the others.
                horizontalPartitionIndex = Mathf.Clamp(horizontalPartitionIndex, 0, horizontalPartitions - 1);
                verticalPartitionIndex = Mathf.Clamp(verticalPartitionIndex, 0, veriticalPartitions - 1);

                brickCounts[horizontalPartitionIndex, verticalPartitionIndex].Item1 += brickIsAlive;
                brickCounts[horizontalPartitionIndex, verticalPartitionIndex].Item2 += 1;
            }
        }

        float[] fractions = new float[horizontalPartitions * veriticalPartitions];
        int fractionsIndex = 0;
        for (int i = 0; i < horizontalPartitions; i++)
        {
            for (int j = 0; j < veriticalPartitions; j++)
            {
                float fractionAlive = (float)brickCounts[i, j].Item1 / brickCounts[i, j].Item2;
                sensor.AddObservation(fractionAlive);
                fractions[fractionsIndex] = fractionAlive;
                fractionsIndex++;
            }
        }
        //Debug.Log("Brick Quadrant Observations: " + string.Join(", ", fractions));
        //Debug.Log("Number of brick status observations: " + horizontalPartitions * veriticalPartitions);

    }

    /// <summary>
    /// Processes the agent's actions and updates the game state accordingly.
    /// Handles paddle movement, episode termination conditions, and reward distribution.
    /// </summary>
    /// <param name="actions">The actions received from the ML-Agent</param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Move the paddle
        float move = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        if (!humanControl)
        {
            rb.linearVelocity = new Vector2(move, 0) * movementSpeed;
        }

        // Conditions for ending the episode
        if (gameManager.IsTrainingMode)
        {
            var score = gameManager.GetScore();
            // end the episode if the agent wins (and there's no more bricks to break)
            if (score == 448)
            {
                EndEpisode();
                return;
            }

            // end the episode if the ball passes the paddle
            if (ball.transform.localPosition.y < gameObject.transform.localPosition.y)
            {
                // Penalize the agent for missing the ball
                SetReward(-1.0f);
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

        // Reward the agent for increasing the score
        float curriculumStage = Academy.Instance.EnvironmentParameters.GetWithDefault("stage", 0);
        if (curriculumStage >= 3.0f)
        {
            newScore = gameManager.GetScore();
            int difference = newScore - prevScore;
            prevScore = newScore;
            if (difference > 0)
            {
                if (curriculumStage == 3.0f)
                {
                    SetReward(1.0f);
                }
                else
                {
                    SetReward(difference * 0.5f);
                }
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

    /// <summary>
    /// Handles collision events between the paddle and other game objects.
    /// Manages rewards and episode termination based on ball collisions.
    /// </summary>
    /// <param name="collision">Information about the collision that occurred</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        float curriculumStage = Academy.Instance.EnvironmentParameters.GetWithDefault("stage", 0);
        if (collision.gameObject.CompareTag("Ball") && (curriculumStage >= 0.0f))
        {
            // In quadrant training mode (stage 3), check if we hit bricks after a paddle collision
            if (curriculumStage == 3.0f)
            {
                bool hitBrickSinceLastBounce = gameManager.GetScore() > score_at_last_bounce;
                score_at_last_bounce = gameManager.GetScore();

                // End the episode if we didn't break any bricks after this paddle bounce
                if (!hitBrickSinceLastBounce)
                {
                    EndEpisode();
                }
            }
            else
            {
                SetReward(0.1f);  // Small reward for each hit
            }
        }
    }
}
