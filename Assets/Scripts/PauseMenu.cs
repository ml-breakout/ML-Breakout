using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;

    public static bool optionsMenuOpen = false;

    public GameObject pauseMenuUI;

    // Use this in scenes that you'd like to scale up or slow down.
    public float timeScale = 1f;

    void Awake()
    {
        Time.timeScale = timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(GamePaused && !optionsMenuOpen){
                Resume();
            }else if(!optionsMenuOpen){
                Pause();
            }
        }
    }

    public void Resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = timeScale;
        GamePaused = false;

    }

    void Pause()
    {
       pauseMenuUI.SetActive(true);
       Time.timeScale = 0f;
       GamePaused = true; 
    }

    public void LoadMainMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void setOptionsMenuOpen(){
        if(optionsMenuOpen){
            optionsMenuOpen = false;
        }else{
            optionsMenuOpen = true;
        }
    }
}
