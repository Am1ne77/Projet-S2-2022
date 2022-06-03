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
        if (this.gameObject.transform.position.y > 4 || this.gameObject.transform.position.y < 0)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
        
        _rigidbody.AddForce(transform.forward * 200);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
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
