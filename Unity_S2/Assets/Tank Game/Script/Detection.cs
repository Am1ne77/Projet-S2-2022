using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Detection : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Found");
            EnemyTank.FoundU(other.transform);
            //Debug.Log("x: " + other.transform.position.x);
            //Debug.Log("y: " + other.transform.position.y);
            //Debug.Log("z: " + other.transform.position.z);
        }
    }
}
