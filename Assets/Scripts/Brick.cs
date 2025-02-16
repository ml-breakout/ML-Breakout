using System;
using UnityEngine;

public class Brick : MonoBehaviour
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

    [SerializeField]
    private int score = 2;
    [SerializeField]
    private string type;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Tuple<int, int> brickCoordinates;
    private GameObject parent;  

    // ****************************
    // * PRIVATE VARIABLES -> END *
    // ****************************
      
    void Start()
    {
        parent = transform.parent.gameObject;
    }

    public void Initialize(Tuple<int, int> coordinates)
    {
        brickCoordinates = coordinates;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (parent != null)
        {
            parent.GetComponent<GameManager>().AddScore(score);
            parent.GetComponent<GameManager>().BrickUpdate(type , brickCoordinates);
        }
        Destroy(this.gameObject);
    }

}
