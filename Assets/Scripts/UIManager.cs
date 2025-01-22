using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void open1PScene()
    {
        SceneManager.LoadScene("SinglePlayer");
    }

    public void open1PAIScene()
    {
        SceneManager.LoadScene("SinglePlayerAI");
    }
    public void openPVPScene()
    {
        SceneManager.LoadScene("PVP");
    }

    public void openPVAIScene(){
        SceneManager.LoadScene("PVAI");
    }
}
