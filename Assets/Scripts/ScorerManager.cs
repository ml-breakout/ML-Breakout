using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.iOS;

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

    public int NumTrials = 10;

    public List<TrialResult> results = new List<TrialResult>();

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

    public void PrintSummaryStats()
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

        Debug.Log("Number of Trials: " + numTrials);
        Debug.Log("Win Rate: " + winRate);
        Debug.Log("Average Score: " + avgScore);
        Debug.Log("Average Bricks Destroyed: " + averageBricksDestroyed);
    }

    public void FinishTrials()
    {    
        PrintSummaryStats();
    }
}