using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Open1PScene()
    {
        SceneManager.LoadScene("SinglePlayer");
    }

    public void Open1PAIScene()
    {
        SceneManager.LoadScene("SinglePlayerAI");
    }
    
    public void OpenPVPScene()
    {
        SceneManager.LoadScene("PVP");
    }

    public void OpenPVAIScene(){
        SceneManager.LoadScene("PVAI");
    }
}
