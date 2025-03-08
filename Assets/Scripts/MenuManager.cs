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

    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************

    void Start()
    {

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
        SceneManager.LoadScene("Scorer");
    }
    
    public void OpenPVPScene()
    {
        SceneManager.LoadScene("PVP");
    }

    public void OpenPVAIScene(){
        SceneManager.LoadScene("PVAI");
    }
    
    public void QuitGame(){
        Application.Quit();
    }

}
