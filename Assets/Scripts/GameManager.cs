using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game
using TMPro;
using System.Runtime.CompilerServices;
using Unity.Mathematics;  // For TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton pattern
    public TextMeshProUGUI scoreText;  // Reference to UI text
    public TextMeshProUGUI livesText;   // Reference to UI text
    public TextMeshProUGUI scoreTextP2;  // Reference to UI text
    public TextMeshProUGUI livesTextP2;   // Reference to UI text
    private int score = 0;
    private int scoreP2 = 0;
    private int lives = 3;
    private int livesp2 = 3;

    public bool MP = true;

    // ball creation vars
    public GameObject ballPrefab;

    private GameObject ballP1;
    private GameObject ballP2;
    private float ballXMP = 4.5f;
    private float ballXSP = 0f;
    private float BallY = -3f;
    // brick creation vars
    public GameObject yellowBrick;
    public GameObject greenBrick;
    public GameObject orangeBrick;
    public GameObject redBrick;

    // for creating bricks be warry of bricks width and height
    [SerializeField]
    private float bricksWidth = 0.5f;
    [SerializeField]
    private float bricksHeight = 0.15f;
    [SerializeField]
    private float playerOffset = 4.5f;
    [SerializeField]
    private float brickSpaceingX = 0.2f;
    [SerializeField]
    private float brickSpaceingY = 0.25f;
    private float bricksInitalX;
    private float bricksInitalY;
    

    void Awake()
    {
        instance = this;
    }

    void Start(){
        createBricks();
        createBalls();
        initLives();
    }

    public void createBricks(){
        GameObject[] bricks = {yellowBrick,greenBrick,orangeBrick,redBrick};
        bricksInitalX = bricksWidth/2 + brickSpaceingX/2;

        if(MP == true){
            for(var k = 0; k < 2; k++){
                for(var i = 0; i < 8; i++){
                    for(var j = 0; j < 7; j++){
                        Instantiate(bricks[(int)math.floor(i/2)], new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
                        Instantiate(bricks[(int)math.floor(i/2)], new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
                    }
                    bricksInitalY += bricksHeight + brickSpaceingY;
                }
                bricksInitalY = 0f;
                playerOffset *= -1;
            }
            
        }else{
            for(var i = 0; i < 2; i++){
                for(var j = 0; j < 7; j++){
                    Instantiate(bricks[(int)math.floor(i/2)], new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
                    Instantiate(bricks[(int)math.floor(i/2)], new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
                }
                bricksInitalY += bricksHeight + brickSpaceingY;
            }
        }
    }

    public void createBalls(){
        if(MP == true){
            ballP1 = Instantiate(ballPrefab, new Vector3(-ballXMP, BallY, 0), Quaternion.identity);
            ballP2 = Instantiate(ballPrefab, new Vector3(ballXMP, BallY, 0), Quaternion.identity);
        }else{
            ballP1 = Instantiate(ballPrefab, new Vector3(ballXSP, BallY, 0), Quaternion.identity);
        }
    }
    public void initLives(){
        if(MP ==false){
            livesText.text = lives.ToString();
        }else{  
            livesText.text = lives.ToString(); 
            livesTextP2.text = livesp2.ToString(); 
        }
    }
    public void GameOver()
    {
        Debug.Log("Game Over!");
        // Restart the game after 2 seconds
        Invoke("RestartGame", 2f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int updateScore, GameObject ballObject)
    {
        if(MP ==false){
            score += updateScore;
            scoreText.text = score.ToString();
        }else{
            if(ballObject == ballP1){
                score += updateScore;
                scoreText.text = score.ToString(); 
            }else if(ballObject == ballP2){
                scoreP2 += updateScore;
                scoreTextP2.text = scoreP2.ToString(); 
            }else{
                Debug.Log(ballObject);
            }
        }
        
    }

    public Vector2 GetBallStartingPosition(GameObject ballObject){
        Vector2 newBallPos;
        if(MP == false){
            newBallPos = new Vector2(ballXSP,BallY);
        }else{
            if(ballObject == ballP1){
                newBallPos = new Vector2(-ballXMP,BallY); 
            }else if(ballObject == ballP2){
                newBallPos = new Vector2(ballXMP,BallY); 
            }else{
                Debug.Log(ballObject);
                newBallPos = new Vector2(0,0);
            }
        }
        return newBallPos;
    }

    public int LoseALife(GameObject ballObject)
    {
        int returnLives;
        if(MP == true){
            // ball check
            if(ballObject == ballP1){
                lives--;
                livesText.text = lives.ToString(); 
                returnLives = lives;
            }else if(ballObject == ballP2){
                livesp2--;
                livesTextP2.text = livesp2.ToString(); 
                returnLives = livesp2;
            }else{
                Debug.Log(ballObject);
                returnLives = 0;
            }
            // game state check
            if (lives <= 0 && livesp2 <= 0)
            {
                GameOver();
            }
        }else{
            // single player handle
            lives--;
            if (lives <= 0)
            {
                GameOver();
            }
            returnLives = lives;
            
        }
        return returnLives;
    }
}