using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game
using TMPro;  // For TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton pattern
    public TextMeshProUGUI scoreText;  // Reference to UI text
    private int score = 0;

    void Awake()
    {
        instance = this;
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

    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }
}