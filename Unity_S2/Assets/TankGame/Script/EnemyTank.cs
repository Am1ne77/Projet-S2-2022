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
    
    public void Awake()
    {
        LastShot = DateTime.Now;
    }
    
    void Update()
    {
        transform.LookAt(target);
        transform.Translate(new Vector3(0,0,(float) 0.15));
        
        TimeSpan ready = DateTime.Now - LastShot;
        if (ready.Seconds >= 3)
        {
            var bull = Instantiate(bullet, this.gameObject.transform.position, this.gameObject.transform.rotation);
            LastShot = DateTime.Now;
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Enemy"))
        {
            transform.Rotate(new Vector3(0,-4,0), Space.Self);
        }

        if (collision.gameObject.CompareTag("Bullet") && (DateTime.Now - LastShot).Milliseconds > 50)
        {
            Destroy(this);
            Destroy(this.gameObject);
            GameController.EnemyDestroyed();
        }
    }

    public static void FoundU(Transform transform)
    {
        target = transform;
    }
    
}
