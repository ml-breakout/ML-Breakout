using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;

[TestFixture]
public class PaddleAgentControllerTests
{
    private GameObject paddleObject;
    private PaddleAgentController paddleAgent;
    private GameObject ballObject;
    private GameManager mockGameManager;
    private GameObject gameManagerObject;
    private Rigidbody2D ballRigidbody;
    private List<float> observations;

    [SetUp]
    public void SetUp()
    {
        gameManagerObject = new GameObject("GameManager");
        mockGameManager = gameManagerObject.AddComponent<GameManager>();
        
        paddleObject = new GameObject("Paddle");
        paddleAgent = paddleObject.AddComponent<PaddleAgentController>();
        paddleAgent.gameManager = mockGameManager;
        paddleAgent.paddleSize = 1.0f;
        
        ballObject = new GameObject("Ball");
        ballObject.tag = "Ball";
        ballRigidbody = ballObject.AddComponent<Rigidbody2D>();
        
        mockGameManager.SetBall(ballObject);
        
        List<List<int>> bricksAlive = new List<List<int>>();
        for (int v = 0; v < 8; v++)
        {
            List<int> row = new List<int>();
            for (int h = 0; h < 7; h++)
            {
                row.Add(1); 
            }
            bricksAlive.Add(row);
        }
        mockGameManager.SetBricksAlive(bricksAlive);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(paddleObject);
        UnityEngine.Object.DestroyImmediate(ballObject);
        UnityEngine.Object.DestroyImmediate(gameManagerObject);
    }

    [Test]
    public void CollectObservations_BallPosition_AddedCorrectly()
    {
        ballObject.transform.localPosition = new Vector3(2.0f, 3.0f, 0);
        
        observations = paddleAgent.GetObservationsForTesting();
        
        float expectedBallX = (2.0f - (-4.5f)) / (4.5f - (-4.5f)); 
        float expectedBallY = (3.0f - (-4.75f)) / (4.75f - (-4.75f)); 
        
        Assert.AreEqual(expectedBallX, observations[0], 0.001f);
        Assert.AreEqual(expectedBallY, observations[1], 0.001f);
    }

    [Test]
    public void CollectObservations_PaddlePosition_AddedCorrectly()
    {
        paddleObject.transform.localPosition = new Vector3(1.5f, 0, 0);
        
        observations = paddleAgent.GetObservationsForTesting();
        
        float expectedPaddleX = (1.5f - (-3.5f)) / (3.5f - (-3.5f)); 
        
        Assert.AreEqual(expectedPaddleX, observations[2], 0.001f);
    }

    [Test]
    public void CollectObservations_BallVelocity_AddedCorrectly()
    {
        ballRigidbody.linearVelocity = new Vector2(3.0f, -2.0f);
        
        observations = paddleAgent.GetObservationsForTesting();

        float expectedVelocityX = 0.6f;
        
        float expectedVelocityY = -0.4f;

        Debug.Log($"Observations length: {observations.Count}");
        for (int i = 0; i < observations.Count; i++)
        {
            Debug.Log($"Observation[{i}] = {observations[i]}");
        }

        Assert.AreEqual(expectedVelocityX, observations[6], 0.001f, "X velocity should be at index 6");
        Assert.AreEqual(expectedVelocityY, observations[7], 0.001f, "Y velocity should be at index 7");
    }

    [Test]
    public void CollectObservations_BrickPartitions_CalculatedCorrectly()
    {
        List<float> initialObservations = paddleAgent.GetObservationsForTesting();
        
        Debug.Log("Initial observations for brick partitions:");
        for (int i = 9; i < initialObservations.Count; i++)
        {
            Debug.Log($"Brick Partition {i-9}: {initialObservations[i]}");
        }
        
        List<List<int>> bricksAlive = mockGameManager.GetBricksAlive();
        
        Debug.Log($"Bricks array dimensions: {bricksAlive.Count} rows x {bricksAlive[0].Count} columns");
        
        for (int v = 0; v < 4; v++)
        {
            for (int h = 0; h < bricksAlive[v].Count; h++)
            {
                if (h % 2 == 0) 
                {
                    bricksAlive[v][h] = 0; 
                    Debug.Log($"Destroyed brick at v={v}, h={h}");
                }
            }
        }
        
        mockGameManager.SetBricksAlive(bricksAlive);
        
        observations = paddleAgent.GetObservationsForTesting();
        
        Debug.Log("Modified observations for brick partitions:");
        for (int i = 9; i < observations.Count; i++)
        {
            Debug.Log($"Brick Partition {i-9}: {observations[i]}");
        }
        
        bool foundModifiedPartition = false;
        for (int i = 9; i < observations.Count; i++)
        {
            if (observations[i] < 0.99f)
            {
                foundModifiedPartition = true;
                Debug.Log($"Found modified partition at index {i} with value {observations[i]}");
                break;
            }
        }
        
        Assert.IsTrue(foundModifiedPartition, "At least one brick partition should have been modified");
    }
} 