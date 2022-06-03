using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using Random = System.Random;

public class EnemyTank : MonoBehaviour
{
    [HideInInspector]
    public GameObject bullet;
    
    private static Transform target;

    private DateTime LastShot;
    
    private static DateTime LastTimeSeen;

    private DateTime LastCollision;

    private Rigidbody _rigidbody;
    
    private Transform ShootPoint;

    public void Awake()
    {
        LastShot = DateTime.Now;
        LastTimeSeen = DateTime.Now;
        _rigidbody = GetComponent<Rigidbody>();
        ShootPoint = this.gameObject.transform.Find("ShootPoint");
        _rigidbody.maxAngularVelocity = 2.0f;
    }

    private void Start()
    {
        LastCollision = DateTime.MinValue;
    }

    void Update()
    {
        //Help Ai if stuck in wall
        if ((DateTime.Now - LastCollision).Seconds <= 2)
        {
            _rigidbody.AddForce(transform.forward * 15);
            return;
        }
        
        //Destroy tanks if the player is destroyed
        if ((DateTime.Now - LastTimeSeen).Seconds > 1)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }

        //Ai looks towards and player and moves
        transform.LookAt(target);
        if (Vector3.Distance(_rigidbody.position, target.transform.position) > 10)
        {
            Debug.Log(1);
            _rigidbody.AddForce(transform.forward * 15);
        }
        
        
        
        //Ai shoot
        TimeSpan ready = DateTime.Now - LastShot;
        if (ready.Seconds >= 3)
        {
            var bull = Instantiate(bullet, ShootPoint.position, this.gameObject.transform.rotation);
            LastShot = DateTime.Now;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Gets destroyed by bullet
        if ((collision.gameObject.CompareTag("Bullet") && (DateTime.Now - LastShot).Milliseconds > 50))
        {
            Destroy(this);
            Destroy(this.gameObject);
            GameController.EnemyDestroyed();
        }

        //Gets destroyed by the player if his speed is greater
        if (collision.gameObject.CompareTag("Player") && collision.rigidbody.velocity.magnitude > _rigidbody.velocity.magnitude)
        {
            Destroy(this);
            Destroy(this.gameObject);
            GameController.EnemyDestroyed();
        }
        
        //Collision between wall and very fast enemy
        if ((collision.gameObject.CompareTag("VerticalWall") || collision.gameObject.CompareTag("HorizontalWall")
            ||collision.gameObject.CompareTag("Walls")) && _rigidbody.velocity.magnitude >= 6)
        {
            Destroy(this);
            Destroy(this.gameObject);
            GameController.EnemyDestroyed();
        }
        
        //Collision between 2 enemies
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _rigidbody.AddForce(- transform.forward * 75);
        }
        
        
        
        //Help Ai in case stuck in wall
        if (collision.gameObject.CompareTag("VerticalWall"))
        {
             LastCollision = DateTime.Now;
             _rigidbody.rotation = new Quaternion(0, 45, 0, 45).normalized;
             //transform.Rotate(new Vector3(0,transform.rotation.y - 90,0));
        }

        if (collision.gameObject.CompareTag("HorizontalWall"))
        {
            LastCollision = DateTime.Now;
            _rigidbody.rotation = new Quaternion(0, 180, 0, 0).normalized;
            //transform.Rotate(new Vector3(0,transform.rotation.y - 180,0));
        }
    }

    public static void FoundU(Transform transform)
    {
        //Instantiate the target which in this the player
        target = transform;
        LastTimeSeen = DateTime.Now;
    }
    
}
