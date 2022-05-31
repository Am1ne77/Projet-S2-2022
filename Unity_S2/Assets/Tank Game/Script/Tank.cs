using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] 
    private GameObject bullet;

    [SerializeField] 
    private GameObject bulletManager;

    private DateTime LastShot;

    private bool goingforward;
    
    

    void Start()
    {
        LastShot = DateTime.Now;
        goingforward = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0,0,(float) 0.2));
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
                var bull = bulletManager.AddComponent<Bullet>();
                bull.Shoot(bullet, transform.position, transform.rotation);
                LastShot = DateTime.Now;
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Walls"))
        {
            if (goingforward)
            {
                transform.Translate(new Vector3(0, 0, -1));
            }
            else
            {
                transform.Translate(new Vector3(0, 0, 1));
            }
        }

    }
}
