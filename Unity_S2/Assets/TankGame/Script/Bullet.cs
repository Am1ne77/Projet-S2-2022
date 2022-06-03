using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private DateTime Shot = DateTime.Now;

    private Rigidbody _rigidbody;

    private void Start()
    {
        gameObject.tag = "Bullet";
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Destroys the bullet if somehow it went over the wall
        if (this.gameObject.transform.position.y > 4 || this.gameObject.transform.position.y < 0)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
        
        //Moves the bullet
        _rigidbody.AddForce(transform.forward * 200);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Bullet gets destroyed on wall
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("VerticalWall")
            || collision.gameObject.CompareTag("HorizontalWall") || collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(this.gameObject);
            Destroy(this);
        }

        //Temp
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
        
    }

    public void Awake()
    {
        Shot = DateTime.Now;
        this.gameObject.tag = "Bullet";
    }

}
