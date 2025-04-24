using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PostGameMenu : MonoBehaviour
{
    public static bool GamePaused = false;

    //public static bool postGameMenuOpen = false;

    public GameObject postGameMenuUI;

    // Update is called once per frame
    // Can be removed after testing is complete
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete)){
            if(GamePaused){
                Resume();
            }else{
                Activate();
            }
        }
    }

    public void Resume(){
        postGameMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;

    }

    public void Activate()
    {
       postGameMenuUI.SetActive(true);
       Time.timeScale = 0f;
       GamePaused = true;

       UnityEngine.SceneManagement.Scene thisScene = SceneManager.GetActiveScene();
       if (thisScene.name == "SinglePlayerAI")
       {
            PlaySceneAgain();
       }
       else if (thisScene.name == "SinglePlayerAITrainer")
       {
            PlaySceneAgain();
       }
       else if (thisScene.name == "18PlayerAITrainer 1")
       {
            PlaySceneAgain();
       }
       else if (thisScene.name == "Scorer")
       {
            PlaySceneAgain();
       }
    }

    public void PlaySceneAgain()
    {
        UnityEngine.SceneManagement.Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisScene.name);
        Resume();
    }
    
    public void QuitGame(){
        Application.Quit();
    }
    
    public void LoadMainMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Resume();
    }
}
