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
    public GameObject bulletManager;

    [HideInInspector]
    public GameObject bullet;

    private GameObject tank;

    public void Spawn(GameObject tankGameObject, Vector3 pos, Quaternion rot, 
        GameObject bulletManagerGameObject, GameObject bulletGameObject)
    {
        tank = Instantiate(tankGameObject, pos, rot);
        LastShot = DateTime.Now;
        this.bulletManager = bulletManagerGameObject;
        this.bullet = bulletGameObject;
    }
    
    void Update()
    {
        
        transform.LookAt(target);
        transform.Translate(new Vector3(0,0,(float) 0.1));
        
        TimeSpan ready = DateTime.Now - LastShot;
        if (ready.Seconds >= 3 && bullet != null)
        {
            LastShot = DateTime.Now;
            var bull = bulletManager.AddComponent<Bullet>();
            bull.Shoot(bullet, this.tank.transform.position, this.tank.transform.rotation);
            Destroy(bulletManager.GetComponent<Bullet>());
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Enemy"))
        {
            transform.Rotate(new Vector3(0,-4,0), Space.Self);
        }
    }

    public static void FoundU(Transform transform)
    {
        target = transform;
    }
}
