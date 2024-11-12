using UnityEngine;
using UnityEngine.SceneManagement;  // For restarting the game

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton pattern

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
}