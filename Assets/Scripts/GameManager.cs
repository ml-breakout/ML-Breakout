using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game
using TMPro;
using Unity.Mathematics;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // UI vars
    // public static GameManager instance;  // Singleton pattern
    public TextMeshProUGUI scoreText;  // Reference to UI text
    public TextMeshProUGUI livesText;   // Reference to UI text

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

    // AI Agent vars
    public bool IsAgentPlayer; // Whether the AI agent is playing the game
    public bool IsTrainingMode = false;  // Whether the game is in training mode

    // paddle vars

    [SerializeField]
    private GameObject paddle;


    // ball creation vars
    public GameObject ballPrefab;
    private GameObject BallObject;
    private Vector2 ballCenter;

    private List<GameObject> currentBricks = new List<GameObject>();

    private List<int> currentBricksAlive = new List<int>();
    // private int[] currentBricksAlive;
    // brick creation vars
    public GameObject yellowBrick;
    public GameObject greenBrick;
    public GameObject orangeBrick;
    public GameObject redBrick;

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


    void Start()
    {
        // Let the Agent initialize the game
        if (!IsAgentPlayer)
        {
            InitializeGame();
        }

        gameStateManager = GameStateManager.instance;
    }

    public void InitializeGame()
    {
        gameCenter = transform.position;
        gameCenter = gameCenter + new Vector2(1.267f, 0.0881f);
        ballCenter = gameCenter + new Vector2(0f, -3f);
        score = 0;
        resetBricks();
        resetBall();
        if (!IsTrainingMode)
        {
            initLives();
            initAudio();
        }

    }

    public void resetBricks()
    {
        Debug.Log(gameCenter);
        // Destroy all current bricks
        foreach (GameObject brick in currentBricks)
        {
            Destroy(brick);
        }
        currentBricks.Clear();
        currentBricksAlive.Clear();

        GameObject[] bricks = { yellowBrick, greenBrick, orangeBrick, redBrick };
        bricksInitalX = bricksWidth / 2 + brickSpaceingX / 2;

        bricksInitalY = 0f;

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 7; j++)
            {
                bricksBuilt++;
                bricksBuilt++;
                Vector3 rightBrickPos = new Vector3((brickSpaceingX + bricksWidth) * j + bricksInitalX + gameCenter.x, bricksInitalY + gameCenter.y, 0);
                Vector3 leftBrickPos = new Vector3((brickSpaceingX + bricksWidth) * -j - bricksInitalX + gameCenter.x, bricksInitalY + gameCenter.y, 0);

                currentBricks.Add(Instantiate(bricks[(int)math.floor(i / 2)], rightBrickPos, Quaternion.identity, this.transform));
                currentBricksAlive.Add(1);
                currentBricks.Add(Instantiate(bricks[(int)math.floor(i / 2)], leftBrickPos, Quaternion.identity, this.transform));
                currentBricksAlive.Add(1);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }

    }

    public void resetBall()
    {
        if (BallObject != null)
        {
            Destroy(BallObject);
        }
        BallObject = Instantiate(ballPrefab, new Vector3(ballCenter.x, ballCenter.y, 0), Quaternion.identity, this.transform);
    }
    public void initLives()
    {
        livesText.text = lives.ToString();
    }
    public void initAudio(){
        AudioSource[] audio_options = GetComponents<AudioSource>();
        foreach(AudioSource source in audio_options)
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
        Debug.Log("Game Over!");
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

    public Vector2 GetBallStartingPosition()
    {
        return ballCenter;
    }

    public int LoseALife()
    {
        if (loseALifeAudioSource != null) {
            loseALifeAudioSource.Play();
        }
        if (IsTrainingMode)
        {
            return 0;  //  Don't lose a life in training mode, let the Agent do it.
        }
        lives--;
        if (lives <= 0)
        {
            if (defeatAudioSource != null) {
                defeatAudioSource.Play();
            }
            gameStateManager.registerGameOver();
        }
        livesText.text = lives.ToString();
        return lives;

    }

    public void BrickUpdate(string type)
    {
        bricksBroken++;

        if (type == "Orange" && !OrangeBrickBroken)
        {
            OrangeBrickBroken = true;
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            Debug.Log("Orange");
        }
        else if (type == "Red" && !RedBrickBroken)
        {
            RedBrickBroken = true;
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            Debug.Log("red");
        }
        if (bricksBroken == 4)
        {
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            Debug.Log("Broken:4");
        }
        else if (bricksBroken == 12)
        {
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            Debug.Log("Broken:12");
        }
        else if (bricksBroken == bricksBuilt)
        {
            victoryAudioSource.Play();
            //until level 2 is added:
            Debug.Log("");
            Invoke("RestartGame", 4f);
            //GameStateManager.instance.checkGameOver();
        }

    }

    public void UpdatePaddleSize()
    {
        if (!IsAgentPlayer)
        {
            paddle.GetComponent<PaddleAgentController>().updatePaddleSize();
        }
    }

    public GameObject GetBall()
    {
        return BallObject;
    }

    public List<int> getBricksAlive(){
        return currentBricksAlive;
    }

}