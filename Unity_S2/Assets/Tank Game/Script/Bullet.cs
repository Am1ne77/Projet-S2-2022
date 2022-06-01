using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private DateTime Shot = DateTime.Now;

    private GameObject bullet;
    
    void Update()
    {
        transform.Translate(new Vector3(0,0,(float) 0.4f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Enemy") && (DateTime.Now - Shot).Milliseconds > 100)
        {
            Destroy(collision.gameObject);
            //Destroy(bullet);
            Destroy(this);
            Destroy(this.gameObject);
            //Tank.TankDestroyed();
        }

        if (collision.gameObject.CompareTag("Walls"))
        {
            //Destroy(bullet);
            Destroy(this);
            Destroy(this.gameObject);
        }
        
    }

    public void Shoot(GameObject bulletGameObject, Vector3 pos, Quaternion rot)
    {
        bullet = Instantiate(bulletGameObject, pos, rot);
        Shot = DateTime.Now;
    }

}
