using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Detection : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        //Finds the player and calls the function to instantiate the variable of enemy tank;
        if (other.CompareTag("Player"))
        {
            EnemyTank.FoundU(other.transform);
        }
    }
}
