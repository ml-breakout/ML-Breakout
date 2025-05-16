using UnityEngine;
using Unity.MLAgents;
using Unity.Sentis;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance; 

    public ModelAsset[] DifficultyModels;

    public int AIDifficulty;

    [Header("Audio")]

    [SerializeField] private AudioClip titleScreenMusic;
    [SerializeField] private AudioClip backgroundMusic;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    

    public void setDifficulty(int difficulty){
        Instance.AIDifficulty = difficulty;
    }

    public ModelAsset getDifficulty(){
        return DifficultyModels[AIDifficulty];
    }

    public int getDifficultyInt(){
        return AIDifficulty;
    }
    void OnSceneChanged(Scene currentScene, Scene nextScene)
    {
        // This code will be executed when a scene change occurs
        // Debug.Log("Scene changed from: " + currentScene.name + " to: " + nextScene.name);

        AudioSource audio = this.GetComponent<AudioSource>();
        if(nextScene.name == "MainMenu"){
            audio.clip = titleScreenMusic;
        }else{
            audio.clip = backgroundMusic;
        }
        audio.Play();

    }

    // scene management stuff
    void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}
