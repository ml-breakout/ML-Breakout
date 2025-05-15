using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    // *****************************
    // * PUBLIC VARIABLES -> START *
    // *****************************

    // ***************************
    // * PUBLIC VARIABLES -> END *
    // ***************************

    // ******************************
    // * PRIVATE VARIABLES -> START *
    // ******************************

    MainManager mainManager;
    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************

    void Start()
    {
        if (MainManager.Instance != null)
        {
            mainManager = MainManager.Instance;
        }
    }

    void Update()
    {

    }

    public void Open1PScene()
    {
        SceneManager.LoadScene("SinglePlayer");
    }

    public void Open1PAIScene()
    {
        if(mainManager.getDifficultyInt() == 3){
            SceneManager.LoadScene("Scorer CNN");
        }else{
            SceneManager.LoadScene("Scorer");
        }
        
    }
    
    public void OpenPVPScene()
    {
        SceneManager.LoadScene("PVP");
    }

    public void OpenPVAIScene(){
        if(mainManager.getDifficultyInt() == 3){
            SceneManager.LoadScene("PVAI CNN"); 
        }else{
            SceneManager.LoadScene("PVAI");
        }
    }
    
    public void QuitGame(){
        Application.Quit();
    }

}
