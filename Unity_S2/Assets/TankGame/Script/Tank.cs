using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Tank : MonoBehaviour
{
    [HideInInspector] 
    public GameObject bullet;

    [HideInInspector] 
    public AudioSource shootsnd;
    
    [HideInInspector] 
    public AudioSource emptyMag;
    
    [HideInInspector] 
    public AudioSource killed;

    private DateTime LastShot;
    
    private Rigidbody _rigidbody;

    private Transform ShootPoint;

    void Awake()
    {
        //Shooting timer
        LastShot = DateTime.Now;
        _rigidbody = GetComponent<Rigidbody>();
        ShootPoint = this.gameObject.transform.Find("ShootPoint");
    }

    void FixedUpdate()
    {
        //check if the player somehow got Out of Bounds
        IsOutOfBounds();

        
        //Forward
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            //transform.Translate(new Vector3(0,0,(float) 0.3));
            _rigidbody.AddForce(transform.forward * 250);
        }
        
        //Backward
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            _rigidbody.AddForce(transform.forward * -250);
        }
        
        //Left
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            _rigidbody.angularVelocity = new Vector3(0, -4, 0);
        }
        
        //Right
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            _rigidbody.angularVelocity = new Vector3(0, 4, 0);
        }

        //Shoot
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TimeSpan ready = DateTime.Now - LastShot;
            if (ready.Seconds >= 1)
            {
                Instantiate(bullet, ShootPoint.position, this.gameObject.transform.rotation);
                LastShot = DateTime.Now;
                Debug.Log("Shoot");
                shootsnd.Play();
            }
            else
            {
                emptyMag.Play();
            }

        }
    }
    

    private void OnCollisionEnter(Collision collisionInfo)
    {
        //Not implemented yet: the player is hit by bullet
        if (collisionInfo.gameObject.CompareTag("Bullet"))
        {
            killed.Play();
            GameController.EndGame();
            Destroy(this.gameObject); 
            Destroy(this);
        }
    }

    private void IsOutOfBounds()
    {
        //Check if player is Out of Bounds to Destroy him
        if (this.transform.position.y < 0)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
    }
    
}
