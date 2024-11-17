using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game
using TMPro;
using System.Runtime.CompilerServices;  // For TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton pattern
    public TextMeshProUGUI scoreText;  // Reference to UI text
    //public TextMeshProUGUI livesText;   // Reference to UI text
    private int score = 0;
    private int lives = 3;


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
    }

    public void createBricks(){
        bricksInitalX = bricksWidth/2 + brickSpaceingX/2;
        // Right player side
        for(var i = 0; i < 2; i++){
            for(var j = 0; j < 7; j++){
                Instantiate(yellowBrick, new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
                Instantiate(yellowBrick, new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
        for(var i = 0; i < 2; i++){
            for(var j = 0; j < 7; j++){
                Instantiate(greenBrick, new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
                Instantiate(greenBrick, new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
        for(var i = 0; i < 2; i++){
            for(var j = 0; j < 7; j++){
                Instantiate(orangeBrick, new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
                Instantiate(orangeBrick, new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
        for(var i = 0; i < 2; i++){
            for(var j = 0; j < 7; j++){
                Instantiate(redBrick, new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
                Instantiate(redBrick, new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX + playerOffset, bricksInitalY, 0), Quaternion.identity);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
        // Left player side
        bricksInitalY = 0f;
        for(var i = 0; i < 2; i++){
            for(var j = 0; j < 7; j++){
                Instantiate(yellowBrick, new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX - playerOffset, bricksInitalY, 0), Quaternion.identity);
                Instantiate(yellowBrick, new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX - playerOffset, bricksInitalY, 0), Quaternion.identity);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
        for(var i = 0; i < 2; i++){
            for(var j = 0; j < 7; j++){
                Instantiate(greenBrick, new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX - playerOffset, bricksInitalY, 0), Quaternion.identity);
                Instantiate(greenBrick, new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX - playerOffset, bricksInitalY, 0), Quaternion.identity);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
        for(var i = 0; i < 2; i++){
            for(var j = 0; j < 7; j++){
                Instantiate(orangeBrick, new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX - playerOffset, bricksInitalY, 0), Quaternion.identity);
                Instantiate(orangeBrick, new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX - playerOffset, bricksInitalY, 0), Quaternion.identity);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
        }
        for(var i = 0; i < 2; i++){
            for(var j = 0; j < 7; j++){
                Instantiate(redBrick, new Vector3((brickSpaceingX + bricksWidth)*j + bricksInitalX - playerOffset, bricksInitalY, 0), Quaternion.identity);
                Instantiate(redBrick, new Vector3((brickSpaceingX + bricksWidth)*-j - bricksInitalX - playerOffset, bricksInitalY, 0), Quaternion.identity);
            }
            bricksInitalY += bricksHeight + brickSpaceingY;
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

    public void AddScore(int updateScore)
    {
        score += updateScore;
        scoreText.text = score.ToString();
    }

    public int LoseALife()
    {
        lives--;
        //livesText.text = "Lives: " + lives;
        // I couldn't get a separate textbox using the same script

        if (lives <= 0)
        {
            GameOver();
        }

        return lives;
    }
}