using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game
using TMPro;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;  // For TextMeshPro

public class NewGameManager : MonoBehaviour
{
    // UI Vars
    public static NewGameManager instance;  // Singleton pattern
    public TextMeshProUGUI[] scoreTextArray;  // Reference to UI text
    public TextMeshProUGUI[] livesTextArray;   // Reference to UI text
    private int[] scoreArray;
    private int[] livesArray;

    // Traning Vars
    public bool trainingMode = false;  // Whether the game is in training mode


    // game handling vars
    public int numPlayers = 1;

    // ball creation vars
    public GameObject ballPrefab;

    private GameObject[] ballArray;

    public Vector2[] ballPosition;
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
    // [SerializeField]
    // private float playerOffset = 4.5f;
    [SerializeField]
    private float brickSpaceingX = 0.2f;
    [SerializeField]
    private float brickSpaceingY = 0.25f;
    public Vector2[] gameCenter; //center of game, used for creating bricks
    

    void Awake()
    {
        instance = this;
    }

    void Start(){
        verfyVariables();
        createBricks();
        createBalls();
        initLives();
    }

    void verfyVariables(){
        // error handling in the arrays, every array should have varibles mathcing numPlayers.
        Debug.Assert(scoreTextArray.Length == numPlayers);
        Debug.Assert(livesTextArray.Length == numPlayers);
        Debug.Assert(ballPosition.Length == numPlayers);
        Debug.Assert(gameCenter.Length == numPlayers);

        // this is done because arrays are locked sizes, thus we are rechanging the sizes once numPlayers is gaurneted.
        scoreArray = new int[numPlayers];
        livesArray = new int[numPlayers];
        ballArray = new GameObject[numPlayers];


        for(var i = 0; i < numPlayers; i++){
            scoreArray[i] = 0;
            livesArray[i] = 3;
        }
    }

    public void createBricks(){
        GameObject[] bricks = {yellowBrick,greenBrick,orangeBrick,redBrick};

        float bricksInitalX = bricksWidth/2 + brickSpaceingX/2; 
        float bricksInitalY = 0f;

        for(var k = 0; k < numPlayers; k++){
            for(var i = 0; i < 8; i++){
                for(var j = 0; j < 7; j++){
                    Instantiate(bricks[(int)math.floor(i/2)], new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX + gameCenter[k].x, bricksInitalY, 0), Quaternion.identity);
                    Instantiate(bricks[(int)math.floor(i/2)], new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX + gameCenter[k].x, bricksInitalY, 0), Quaternion.identity);
                }
                bricksInitalY += bricksHeight + brickSpaceingY;
            }
            bricksInitalY = 0f;
        }
            
        
    }

    public void createBalls(){
        if (trainingMode)
        {
            return;  //  Don't create a ball in training mode, let the Agent do it.
        }
        for(var i = 0; i < numPlayers; i++){
            ballArray[i] = Instantiate(ballPrefab, new Vector3(ballPosition[i].x, ballPosition[i].y, 0), Quaternion.identity);
        }
    }
    public void initLives(){
        for(var i = 0; i < numPlayers; i++){
            livesTextArray[i].text = livesArray[i].ToString();
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
        for(var i = 0; i < numPlayers; i++){
            Debug.Log(ballArray[i] == ballObject);
            if(ballArray[i] == ballObject){
                scoreArray[i] += updateScore;
                scoreTextArray[i].text = scoreArray[i].ToString();
            }
        }
        
    }

    public Vector2 GetBallStartingPosition(GameObject ballObject){
        Vector2 newBallPos = new Vector2(0,0);
        for(var i = 0; i < numPlayers; i++){
            if(ballArray[i] == ballObject){
                newBallPos = ballPosition[i];
            }
        }
        return newBallPos;
    }

    public int LoseALife(GameObject ballObject)
    {
        if (trainingMode)
        {
            return 0;  //  Don't lose a life in training mode, let the Agent do it.
        }
        int returnLives = 0;
        for(var i = 0; i < numPlayers; i++){
            if(ballArray[i] == ballObject){
                livesArray[i]--;
                livesTextArray[i].text = livesArray[i].ToString();
                returnLives = livesArray[i];
            }
        }
        if(checkGameOver()){
            GameOver();
        }
        return returnLives;
    }

    bool checkGameOver(){
        for(var i = 0; i < numPlayers; i++){
            if(livesArray[i] > 0){
                return false;
            }
        }
        return true;
    }
}