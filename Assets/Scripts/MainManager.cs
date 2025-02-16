using UnityEngine;
using Unity.MLAgents;
using Unity.Sentis;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance; 

    public ModelAsset[] DifficultyModels;

    public int AIDifficulty;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void setDifficulty(int difficulty){
        MainManager.Instance.AIDifficulty = difficulty;
    }

    public ModelAsset getDifficulty(){
        return DifficultyModels[AIDifficulty];
    }
}
