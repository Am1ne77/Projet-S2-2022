using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start()
    {
    }

    
    void Update()
    {
        transform.Translate(new Vector3(0,0,(float) 1));
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Walls"))
        {
            Destroy(this);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject.gameObject);
            Destroy(this);
            Destroy(this.gameObject);
        }

    }
    
    public void Shoot(GameObject bulletGameObject, Vector3 pos, Quaternion rot)
    {
        Instantiate(bulletGameObject, pos, rot);
    }
    
}
