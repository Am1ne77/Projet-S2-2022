using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private DateTime Shot = DateTime.Now;

    private void Start()
    {
        gameObject.tag = "Bullet";
    }

    void Update()
    {
        transform.Translate(new Vector3(0,0,(float) 0.4f));
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
