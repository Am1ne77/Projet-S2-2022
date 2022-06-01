using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using Random = System.Random;

public class EnemyTank : MonoBehaviour
{

    private bool tryleft;

    private DateTime LastCol;

    private static Transform target;
    

    public void Spawn(GameObject bulletGameObject, Vector3 pos, Quaternion rot)
    {
        Instantiate(bulletGameObject, pos, rot);
        Random r = new Random();
        tryleft = r.Next(0, 2) == 0;
        LastCol = DateTime.Now;
    }
    
    void Update()
    {
        TimeSpan swi = DateTime.Now - LastCol;
        if (swi.Seconds >= 2)
        {
            tryleft = !tryleft;
        }
        transform.LookAt(target);
        //transform.Translate(new Vector3(0,0,(float) 0.13));
        
    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Enemy"))
        {
            if (tryleft)
            {
                transform.Rotate(new Vector3(0,-4,0), Space.Self);
            }
            else
            {
                transform.Rotate(new Vector3(0,4,0), Space.Self);
            }
            
            LastCol = DateTime.Now;
        }
    }

    public static void FoundU(Transform transform)
    {
        //Debug.Log("Look");
        target = transform;
        //Debug.Log("x: " + other.transform.position.x);
        //Debug.Log("y: " + other.transform.position.y);
        //Debug.Log("z: " + other.transform.position.z);
    }
}
