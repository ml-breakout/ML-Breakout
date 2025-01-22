using System;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Brick : MonoBehaviour
{   
    [SerializeField]
    private int score = 2;

    [SerializeField]
    private string type;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private int ID;

    private GameObject parent;    
    void Start(){
        parent = transform.parent.gameObject;

        ID = this.gameObject.GetInstanceID();
    }

    private void OnCollisionEnter2D(Collision2D collision){
        // Debug.Log(collision.gameObject);
        if (parent != null)
        {
            parent.GetComponent<GameManager>().AddScore(score);
            parent.GetComponent<GameManager>().BrickUpdate(type , ID);
        }
        Destroy(this.gameObject);
    }
}
