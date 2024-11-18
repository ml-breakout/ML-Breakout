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

    public void open1PScene(){
        SceneManager.LoadScene(1);
    }
    public void openPVPScene(){
        SceneManager.LoadScene(2);
    }
    // need to finish
    // public void openPVAIScene(){
    //     SceneManager.GetSceneByBuildIndex(2);
    // }
}
