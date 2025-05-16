using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class ScorerManager : MonoBehaviour
{

    public class TrialResult
    {
        public bool success;
        public float time;
        public int score;
        public int bricksDestroyed;

        public int bounces;
    }

    public int NumTrials = 4;

    public List<TrialResult> results = new List<TrialResult>();

    public TextMeshProUGUI ScorerResultsText;

    public void RegisterTrialResult(bool success, float time, int score, int bounces, int bricksDestroyed)
    {

        results.Add(new TrialResult
        {
            success = success,
            time = time,
            score = score,
            bounces = bounces,
            bricksDestroyed = bricksDestroyed
        });

        NumTrials--;
        if (NumTrials == 0)
        {
            FinishTrials();
        }
    }

    public string GetSummaryStats()
    {
        int numTrials = results.Count;
        int winCount = 0;
        foreach (var result in results) {
            if (result.success) {
                winCount++;
            }
        }
        float winRate = (float)winCount / numTrials;

        int totalScore = 0;
        foreach (var result in results) {
            totalScore += result.score;
        }
        float avgScore = (float)totalScore / numTrials;

        int totalBricksDestroyed = 0;
        foreach (var result in results) {
            totalBricksDestroyed += result.bricksDestroyed;
        }
        float averageBricksDestroyed = (float)totalBricksDestroyed / numTrials;

        float totalTime = 0f;
        foreach (var result in results) {
            if (result.success) {
                totalTime += result.time;
            }
        }
        float avgTime;
        if (winCount > 0) {
            avgTime = totalTime / winCount;
        } else {
            avgTime = float.MaxValue; // Avoid division by zero if no wins
        }

        string res = "Number of Trials: " + numTrials + "\n" +
                     "Win Rate: " + winRate + "\n" +
                     "Average Score: " + avgScore + "\n" +
                     "Average Time: " + avgTime + "\n";

        return res;
    }

    public void FinishTrials()
    {    
        PostGameMenu menu = GameObject.FindWithTag("PostGameMenuUI").GetComponent<PostGameMenu>();
        menu.Activate("Finished Trials.");
        ScorerResultsText.text = GetSummaryStats();
    }
}