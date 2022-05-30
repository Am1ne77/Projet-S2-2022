using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject bullet;
    public void Shoot(GameObject bulletGameObject, Vector3 pos, Quaternion rot)
    {
        var bul = Instantiate(bulletGameObject, pos, rot);
        this.bullet = bul;
    }
    
    void Start()
    {
        
    }

    
    void Update()
    {
        transform.Translate(new Vector3(0,0,(float) 0.3));
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Walls"))
        {
            if (bullet is null)
            {
                var body = GetComponent<Rigidbody>();
                body.constraints = RigidbodyConstraints.FreezeAll;
                Destroy(this);
            }
            
        }

    }
    
}
