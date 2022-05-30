using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject bullet;

    private Rigidbody _rigidbody;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        transform.Translate(new Vector3(0,0,(float) 1));
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Walls"))
        {
            if (bullet is null)
            {
                transform.Translate(new Vector3(0,-10,0));
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                Destroy(this);
            }
            
        }

    }
    
    public void Shoot(GameObject bulletGameObject, Vector3 pos, Quaternion rot)
    {
        var bul = Instantiate(bulletGameObject, pos, rot);
        this.bullet = bul;
    }
    
}
