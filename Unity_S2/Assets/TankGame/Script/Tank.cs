using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using Random = System.Random;

public class Tank : MonoBehaviour
{
    [HideInInspector] 
    public GameObject bullet;

    private DateTime LastShot;

    private bool goingforward;

    void Awake()
    {
        //Shooting timer
        LastShot = DateTime.Now;
        goingforward = true;
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0,0,(float) 0.3));
            goingforward = true;
        }
        
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0,0,(float) -0.2));
            goingforward = false;
        }
        
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0,-4,0), Space.Self);
        }
        
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0,4,0), Space.Self);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TimeSpan ready = DateTime.Now - LastShot;
            if (ready.Seconds >= 3)
            {
                var bull = Instantiate(bullet, this.gameObject.transform.position, this.gameObject.transform.rotation);
                LastShot = DateTime.Now;
            }
           
        }
    }
    

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Walls"))
        {
            if (goingforward)
            {
                transform.Translate(new Vector3(0, 0, -2));
            }
            else
            {
                transform.Translate(new Vector3(0, 0, 2));
            }
        }

        if (collisionInfo.gameObject.CompareTag("Enemy"))
        {
            transform.Translate(new Vector3(0, 0, -2));
        }
    }
    
}
