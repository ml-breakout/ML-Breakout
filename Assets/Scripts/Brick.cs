using Unity.VisualScripting;
using UnityEngine;

public class Brick : MonoBehaviour
{   
    [SerializeField]
    private int score = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter2D(Collision2D collision){
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(score);
        }
        Destroy(this.gameObject);
    }
}
