using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game
using TMPro;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Timers;
using System.IO;

public class GameManager : MonoBehaviour
{
    // *****************************
    // * PUBLIC VARIABLES -> START *
    // *****************************

    // UI vars
    public TextMeshProUGUI scoreText;  // Reference to UI text
    public TextMeshProUGUI livesText;   // Reference to UI text
    public TextMeshProUGUI averageScore;   // Reference to UI text
    public TextMeshProUGUI gamesPlayed;   // Reference to UI text

    // AI Agent vars
    public bool IsAgentPlayer; // Whether the AI agent is playing the game
    public bool IsTrainingMode = false;  // Whether the game is in training mode
    public bool IsScoringPlayer = false;

    // Ball Creation
    public GameObject ballPrefab;

    // brick creation vars
    public GameObject yellowBrick;
    public GameObject greenBrick;
    public GameObject orangeBrick;
    public GameObject redBrick;

    // Scorer Variables
    public TextMeshProUGUI bouncesText;
    public TextMeshProUGUI timerText;

    // ***************************
    // * PUBLIC VARIABLES -> END *
    // ***************************


    // ******************************
    // * PRIVATE VARIABLES -> START *
    // ******************************

    // game managment vars
    private int score = 0;
    private int lives = 3;
    private Vector2 gameCenter;
    private int bricksBroken;
    private int bricksBuilt;
    private bool OrangeBrickBroken = false;
    private bool RedBrickBroken = false;
    [SerializeField]
    private float speedChange = 1f;

    // paddle vars
    [SerializeField]
    private GameObject paddle;

    // ball creation vars
    private GameObject BallObject;
    private Vector2 ballCenter;
    private List<List<GameObject>> currentBricks = new List<List<GameObject>>();
    private List<List<int>> currentBricksAlive = new List<List<int>>();
    [SerializeField]
    private float bricksWidth = 0.5f;
    [SerializeField]
    private float bricksHeight = 0.15f;
    [SerializeField]
    private float brickSpaceingX = 0.15f;
    [SerializeField]
    private float brickSpaceingY = 0.25f;
    private float bricksInitalX;
    private float bricksInitalY = 0f;
    [SerializeField] private AudioClip loseALifeAudioClip;
    [SerializeField] private AudioClip defeatAudioClip;
    [SerializeField] private AudioClip victoryAudioClip;


    private GameStateManager gameStateManager;
    private int Bounces;
    private float CurrentTime;
    private bool HasLost = false;
    private bool HasWon = false;

    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************
    
    void Start()
    {
        // Let the Agent initialize the game
        if (!IsAgentPlayer)
        {
            InitializeGame();
        }

        gameStateManager = GameStateManager.instance;
        Bounces = 0;        
    }

    public void InitializeGame()
    {
        gameCenter = transform.position;
        gameCenter = gameCenter + new Vector2(0f, 0f);
        ballCenter = gameCenter + new Vector2(0f, -3f);
        score = 0;
        scoreText.text = score.ToString();
        ResetBricks();
        ResetBall();
        if (!IsTrainingMode)
        {
            InitLives();
        }
        if (IsScoringPlayer)
        {
            CurrentTime = 0;
            SetBouncesText();
            SetTimerText();
        }        
    }

    public void ResetBricks()
    {
        // Destroy all current bricks
        foreach (List<GameObject> brickRow in currentBricks)
        {
            foreach (GameObject brick in brickRow)
            {
                Destroy(brick);
            }
        }
        currentBricks.Clear();
        currentBricksAlive.Clear();
        bricksBuilt = 0;

        GameObject[] brickColors = { yellowBrick, greenBrick, orangeBrick, redBrick };
        bricksInitalX = bricksWidth / 2 + brickSpaceingX / 2;

        bricksInitalY = 0f;
        float topmostYPosition = gameCenter.y;
        float leftmostXPosition = gameCenter.x - 7 * (bricksWidth + brickSpaceingX) + (bricksWidth / 2 + brickSpaceingX / 2);

        for (int verticalIndex = 0; verticalIndex < 8; verticalIndex++)
        {
            // Set up data scructures for tracking the bricks
            List<GameObject> brickRow = new List<GameObject>();
            currentBricks.Add(brickRow);
            List<int> currentBricksAliveRow = new List<int>();
            currentBricksAlive.Add(currentBricksAliveRow);

            float yPosition = topmostYPosition + (verticalIndex * (bricksHeight + brickSpaceingY));
            for (int horizontalIndex = 0; horizontalIndex < 14; horizontalIndex++)
            {
                Tuple<int, int> brickCoordinates = new Tuple<int, int>(horizontalIndex, verticalIndex);
                bricksBuilt++;
                float xPosition = leftmostXPosition + (horizontalIndex * (bricksWidth + brickSpaceingX));
                Vector3 brickPosition = new(xPosition, yPosition, 0);

                GameObject brick = Instantiate(brickColors[(int)math.floor(verticalIndex / 2)], brickPosition, Quaternion.identity, this.transform);
                brick.GetComponent<Brick>().Initialize(brickCoordinates);
                brickRow.Add(brick);

                currentBricksAliveRow.Add(1);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
    }
    
    public void KeepOnlyOneQuadrant()
    {
        // Define the quadrants (assuming 8 rows and 14 columns)
        // The brick grid is divided into 4 quadrants: top-left, top-right, bottom-left, bottom-right
        int verticalMidpoint = currentBricks.Count / 2;    // 4
        int horizontalMidpoint = currentBricks[0].Count / 2;  // 7
        
        // Randomly choose which quadrant to keep
        int quadrant = UnityEngine.Random.Range(0, 4);
        
        // Calculate the vertical and horizontal ranges for the chosen quadrant
        int minVertical, maxVertical, minHorizontal, maxHorizontal;
        
        // Define the quadrant boundaries
        switch (quadrant)
        {
            case 0: // Top-left
                minVertical = 0;
                maxVertical = verticalMidpoint;
                minHorizontal = 0;
                maxHorizontal = horizontalMidpoint;
                break;
            case 1: // Top-right
                minVertical = 0;
                maxVertical = verticalMidpoint;
                minHorizontal = horizontalMidpoint;
                maxHorizontal = currentBricks[0].Count;
                break;
            case 2: // Bottom-left
                minVertical = verticalMidpoint;
                maxVertical = currentBricks.Count;
                minHorizontal = 0;
                maxHorizontal = horizontalMidpoint;
                break;
            case 3: // Bottom-right
                minVertical = verticalMidpoint;
                maxVertical = currentBricks.Count;
                minHorizontal = horizontalMidpoint;
                maxHorizontal = currentBricks[0].Count;
                break;
            default: // Fallback to the full top-left quadrant
                minVertical = 0;
                maxVertical = verticalMidpoint;
                minHorizontal = 0;
                maxHorizontal = horizontalMidpoint;
                break;
        }
        
        // Counter for bricks we're keeping
        int bricksKept = 0;
        
        // Destroy all bricks outside the chosen quadrant
        for (int v = 0; v < currentBricks.Count; v++)
        {
            for (int h = 0; h < currentBricks[v].Count; h++)
            {
                // If the brick is outside our chosen quadrant
                if (v < minVertical || v >= maxVertical || h < minHorizontal || h >= maxHorizontal)
                {
                    if (currentBricks[v][h] != null)
                    {
                        Destroy(currentBricks[v][h]);
                        currentBricksAlive[v][h] = 0;
                    }
                }
                else
                {
                    // Count the bricks we're keeping
                    if (currentBricks[v][h] != null && currentBricksAlive[v][h] == 1)
                    {
                        bricksKept++;
                    }
                }
            }
        }
        
        // Update bricksBuilt to the number of bricks we kept
        bricksBuilt = bricksKept;
        
        Debug.Log($"Kept quadrant {quadrant} with {bricksKept} bricks.");
    }

    public void ResetBall()
    {
        if (BallObject != null)
        {
            Destroy(BallObject);
        }
        BallObject = Instantiate(ballPrefab, new Vector3(ballCenter.x, ballCenter.y, 0), Quaternion.identity, this.transform);
    }

    public void InitLives()
    {
        livesText.text = lives.ToString();
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int updateScore)
    {
        score += updateScore;
        scoreText.text = score.ToString();
    }

    public int GetScore()
    {
        return score;
    }

    public Vector2 GetBallStartingPosition()
    {
        return ballCenter;
    }

    public int LoseALife()
    {
        if (loseALifeAudioClip != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(loseALifeAudioClip, transform, 1f);
        }
        if (IsTrainingMode)
        {
            score = 0;
            return 0;  //  Don't lose a life in training mode, let the Agent do it.
        }
        lives--;
        if (lives <= 0 && !IsScoringPlayer)
        {
            if (defeatAudioClip != null)
            {
                SoundFXManager.instance.PlaySoundFXClip(defeatAudioClip, transform, 1f);
            }
            gameStateManager.RegisterGameOver();
        }
        livesText.text = lives.ToString();
        return lives;

    }

    public void BrickUpdate(string type, Tuple<int, int> brickCoordinates)
    {
        bricksBroken++;

        currentBricksAlive[brickCoordinates.Item2][brickCoordinates.Item1] = 0;

        if (type == "Orange" && !OrangeBrickBroken)
        {
            OrangeBrickBroken = true;
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
        }
        else if (type == "Red" && !RedBrickBroken)
        {
            RedBrickBroken = true;
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
        }
        
        if (bricksBroken == 4)
        {
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
        }
        else if (bricksBroken == 12)
        {
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
        }
        else if (bricksBroken == bricksBuilt)
        {
            SoundFXManager.instance.PlaySoundFXClip(victoryAudioClip, transform, 1f);
            //until level 2 is added:
            if (!IsScoringPlayer && !IsTrainingMode)
            {
                Invoke("RestartGame", 4f);
            }            
            //GameStateManager.instance.checkGameOver();
        }

    }

    public void UpdatePaddleSize()
    {
        paddle.GetComponent<PaddleAgentController>().updatePaddleSize();
    }

    public GameObject GetBall()
    {
        return BallObject;
    }

    public List<List<int>> GetBricksAlive()
    {
        return currentBricksAlive;
    }

    void SetBouncesText()
    {
        if (bouncesText is not null) bouncesText.text = "Paddle Bounces: " + Bounces.ToString();
    }

    public void IncrementBounces()
    {
        if (bricksBroken != bricksBuilt)
        {
            Bounces++;
            SetBouncesText();
        }
    }  

    void Update()
    {
        SetTimerText();
    }
    
    public void SetTimerText()
    {
        if (timerText is not null)
        {        
            if (!HasWon && !HasLost)
            {
                CurrentTime += Time.deltaTime;
                timerText.text = $"Timer: {CurrentTime.ToString("#.00")} seconds";
            }            

            if (lives < 1 && !HasLost)
            {
                HasLost = true;
                float looseTime = CurrentTime;
                string text = $"Agent took {looseTime.ToString("#.00")} seconds to loose the game";
                timerText.text = text;
                text += $", and bounced the ball {Bounces} times, on {DateTime.Now}";
                LogText(text);
            }
            else if (score > 447 && !HasWon)
            {
                HasWon = true;
                float winTime = CurrentTime;
                string text = $"Agent took {winTime.ToString("#.00")} seconds to win the game";
                timerText.text = text;
                text += $", and bounced the ball {Bounces} times on {DateTime.Now}";
                LogText(text);
            }
        }        
    }

    private void OnTimedEvent(System.Object source, ElapsedEventArgs e)
    {
        if (timerText is not null)
        {
            timerText.text = "Timer: " + e.SignalTime;
        }
    }

    public void LogText(string text)
    {
        string directory = Directory.GetCurrentDirectory();
        string fullPath = directory + "\\Assets\\Logs\\Logs.txt";
        using (StreamWriter writer = new(fullPath, true))
        {
            writer.WriteLine(text);
        }
    }

    public bool AllBricksBroken()
    {
        return bricksBuilt > 0 && bricksBroken >= bricksBuilt;
    }
}