using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using Random = System.Random;

public class EnemyTank : MonoBehaviour
{
    
    private static Transform target;

    private DateTime LastShot;
    
    [HideInInspector]
    public GameObject bullet;

    private static DateTime LastTimeSeen;
    
    private Rigidbody _rigidbody;
    
    private Transform ShootPoint;
    
    public void Awake()
    {
        LastShot = DateTime.Now;
        LastTimeSeen = DateTime.Now;
        _rigidbody = GetComponent<Rigidbody>();
        ShootPoint = this.gameObject.transform.Find("ShootPoint");
    }
    
    void Update()
    {
        if ((DateTime.Now - LastTimeSeen).Seconds > 1)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }

        transform.LookAt(target);
        _rigidbody.AddForce(transform.forward * 15);
        
        TimeSpan ready = DateTime.Now - LastShot;
        if (ready.Seconds >= 3)
        {
            var bull = Instantiate(bullet, ShootPoint.position, this.gameObject.transform.rotation);
            LastShot = DateTime.Now;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Bullet") && (DateTime.Now - LastShot).Milliseconds > 50) 
            || collision.gameObject.CompareTag("Player"))
        {
            Destroy(this);
            Destroy(this.gameObject);
            GameController.EnemyDestroyed();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _rigidbody.AddForce(- transform.forward * 75);
        }
    }

    public static void FoundU(Transform transform)
    {
        target = transform;
        LastTimeSeen = DateTime.Now;
    }
    
}
