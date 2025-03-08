using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{

    // *****************************
    // * PUBLIC VARIABLES -> START *
    // *****************************

    public static GameStateManager instance;
    public int numPlayers = 1;
    public PostGameMenu tempObject;

    // ***************************
    // * PUBLIC VARIABLES -> END *
    // ***************************

    // ******************************
    // * PRIVATE VARIABLES -> START *
    // ******************************

    private int numCalledGameOver = 0;

    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************


    void Awake()
    {
        instance = this;
    }

    public void RegisterGameOver(){
        numCalledGameOver++;
        if(numCalledGameOver == numPlayers){
            // Restart the game after 2 seconds
            Invoke("RestartGame", 2f);
        }
    }

    void RestartGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        tempObject = GameObject.FindWithTag("PostGameMenuUI").GetComponent<PostGameMenu>();
        tempObject.Activate();
    }
    
    // TODO
    void CheckNextGame()
    {

    }

}
