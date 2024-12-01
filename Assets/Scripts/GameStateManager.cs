using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager instance;
    public int numPlayers = 1;

    private int numCalledGameOver = 0;



    void Awake(){
        instance = this;
    }

    public void checkGameOver(){
        numCalledGameOver++;
        if(numCalledGameOver == numPlayers){
            Debug.Log("Game Over!");
            // Restart the game after 2 seconds
            Invoke("RestartGame", 2f);
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // TODO
    void checkNextGame(){

    }
}
