using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game
using TMPro;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

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

    // Ball Creation
    public GameObject ballPrefab;

        // brick creation vars
    public GameObject yellowBrick;
    public GameObject greenBrick;
    public GameObject orangeBrick;
    public GameObject redBrick;

    // Scorer Variables
    public TextMeshProUGUI bouncesText;

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
    private string loseALifeAudioName = "173958__leszek_szary__failure";
    private string defeatAudioName = "538151__fupicat__8bit-fall";
    private string victoryAudioName = "752857__etheraudio__square-nice-arpeggio-slow-echo";
    private AudioSource loseALifeAudioSource;
    private AudioSource defeatAudioSource;
    private AudioSource victoryAudioSource;
    private GameStateManager gameStateManager;
    private int Bounces;

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
        SetBouncesText();
    }

    public void InitializeGame()
    {
        gameCenter = transform.position;
        gameCenter = gameCenter + new Vector2(1.267f, 0.0881f);
        ballCenter = gameCenter + new Vector2(0f, -3f);
        score = 0;
        scoreText.text = score.ToString();
        ResetBricks();
        ResetBall();
        if (!IsTrainingMode)
        {
            InitLives();
            InitAudio();
        }
    }

    public void ResetBricks()
    {
        // Debug.Log(gameCenter);
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

    public void InitAudio()
    {
        AudioSource[] audio_options = GetComponents<AudioSource>();
        foreach (AudioSource source in audio_options)
        {
            if (string.Equals(source.clip.name, loseALifeAudioName) is true)
            {
                loseALifeAudioSource = source;
            }
            if (string.Equals(source.clip.name, defeatAudioName) is true)
            {
                defeatAudioSource = source;
            }
            if (string.Equals(source.clip.name, victoryAudioName) is true)
            {
                victoryAudioSource = source;
            }
        }
    }

    public void GameOver()
    {
        //defeatAudioSource.Play();
        // Debug.Log("Game Over!");
        // Restart the game after 2 seconds
        Invoke("RestartGame", 2f);
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
        if (loseALifeAudioSource != null)
        {
            loseALifeAudioSource.Play();
        }
        if (IsTrainingMode)
        {
            score = 0;
            return 0;  //  Don't lose a life in training mode, let the Agent do it.
        }
        lives--;
        if (lives <= 0)
        {
            if (defeatAudioSource != null)
            {
                defeatAudioSource.Play();
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
            // Debug.Log("Orange");
        }
        else if (type == "Red" && !RedBrickBroken)
        {
            RedBrickBroken = true;
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            // Debug.Log("red");
        }
        // Debug.Log(currentBricks.Count);


        // string currentBricksAliveArray = "";
        // for(int i = 0; i < currentBricksAlive.Count; i++){
        //     currentBricksAliveArray += currentBricksAlive[i];
        // }
        // Debug.Log(currentBricksAliveArray);
        if (bricksBroken == 4)
        {
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            // Debug.Log("Broken:4");
        }
        else if (bricksBroken == 12)
        {
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            // Debug.Log("Broken:12");
        }
        else if (bricksBroken == bricksBuilt)
        {
            victoryAudioSource.Play();
            //until level 2 is added:
            // Debug.Log("");
            Invoke("RestartGame", 4f);
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
        Bounces++;
        SetBouncesText();
    }    
}