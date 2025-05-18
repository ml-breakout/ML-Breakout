using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using System;

public class PostGameMenu : MonoBehaviour
{
    public TextMeshProUGUI ResultsText;  // Reference to UI text
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
                Activate("PGM via DELETE KEY");
            }
        }
    }

    public void Resume(){
        postGameMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;

    }

    public void Activate(string results)
    {
        //ResultsText.text = "DEFEAT YOU LOST";
        ResultsText.text = results;
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
        else if (thisScene.name == "SinglePlayerAICNN")
        {
                PlaySceneAgain();
        }
        else if (thisScene.name == "CNN18PlayerAITrainer")
        {
                PlaySceneAgain();
        }
        else if (thisScene.name == "CNNSinglePlayerAITrainer")
        {
                PlaySceneAgain();
        }
        //    else if (thisScene.name == "Scorer CNN")
        //    {
        //         PlaySceneAgain();
        //    }
        // Scorer just loops infinitely if this is uncommented (tested on non-cnn scorer)
    }

    public void PlaySceneAgain()
    {
        UnityEngine.SceneManagement.Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisScene.name);
        Resume();
    }
    
    public void QuitGame(){
        // doesn't work?
        Application.Quit();
    }
    
    public void LoadMainMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Resume();
    }
}
