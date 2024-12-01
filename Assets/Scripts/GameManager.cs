using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game
using TMPro;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using System;  // For TextMeshPro

public class GameManager : MonoBehaviour
{   
    // UI vars
    // public static GameManager instance;  // Singleton pattern
    public TextMeshProUGUI scoreText;  // Reference to UI text
    public TextMeshProUGUI livesText;   // Reference to UI text

    // game managment vars
    private int score = 0;
    private int lives = 3;
    public Vector2 gameCenter;
    private int bricksBroken;
    private int bricksBuilt;
    private bool OrangeBrickBroken = false;
    private bool RedBrickBroken = false;
    [SerializeField]
    private float speedChange = 1f;

    // traning vars
    public bool trainingMode = false;  // Whether the game is in training mode

    // paddle vars

    [SerializeField]
    private GameObject paddle;
  

    // ball creation vars
    public GameObject ballPrefab;
    private GameObject BallObject;
    [SerializeField]
    private Vector2 ballCenter;


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
    

    void Start(){
        createBricks();
        createBalls();
        initLives();
        initAudio();
    }

    public void createBricks(){
        GameObject[] bricks = {yellowBrick,greenBrick,orangeBrick,redBrick};
        bricksInitalX = bricksWidth/2 + brickSpaceingX/2;

        for(var i = 0; i < 8; i++){
            for(var j = 0; j < 7; j++){
                bricksBuilt++;
                bricksBuilt++;
                Vector3 rightBrickPos = new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX + gameCenter.x, bricksInitalY + gameCenter.y, 0);
                Vector3 leftBrickPos = new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX + gameCenter.x, bricksInitalY + gameCenter.y, 0);
                Instantiate(bricks[(int)math.floor(i/2)], rightBrickPos, Quaternion.identity,this.transform);
                Instantiate(bricks[(int)math.floor(i/2)], leftBrickPos, Quaternion.identity,this.transform);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
        
    }

    public void createBalls(){
        if (trainingMode)
        {
            return;  //  Don't create a ball in training mode, let the Agent do it.
        }
        BallObject = Instantiate(ballPrefab, new Vector3(ballCenter.x, ballCenter.y, 0), Quaternion.identity,this.transform);
    }
    public void initLives(){
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

    public Vector2 GetBallStartingPosition(){
        return ballCenter;
    }

    public int LoseALife()
    {
        loseALifeAudioSource.Play();
        if (trainingMode)
        {
            return 0;  //  Don't lose a life in training mode, let the Agent do it.
        }
        lives--;
        if(lives <= 0){
            defeatAudioSource.Play();
            GameStateManager.instance.checkGameOver();
        }
        livesText.text = lives.ToString();
        return lives;

    }

    public void BrickUpdate(string type){
        bricksBroken++;
        
        if(type == "Orange" && !OrangeBrickBroken)
        {
            OrangeBrickBroken = true;
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            Debug.Log("Orange");
        }else if(type == "Red" && !RedBrickBroken)
        {
            RedBrickBroken = true;
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            Debug.Log("red");
        }
        if(bricksBroken == 4)
        {
            BallObject.GetComponent<BallController>().IncreaseBallSpeed(speedChange);
            Debug.Log("Broken:4");
        }else if(bricksBroken == 12)
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

    public void UpdatePaddleSize(){
        paddle.GetComponent<PaddleController>().updatePaddleSize();
    }

}