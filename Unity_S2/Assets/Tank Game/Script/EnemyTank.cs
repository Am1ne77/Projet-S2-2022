using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemyTank : MonoBehaviour
{

    private bool tryleft;

    private DateTime LastCol;
    
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
        
        transform.Translate(new Vector3(0,0,(float) 0.13));
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
    
}
