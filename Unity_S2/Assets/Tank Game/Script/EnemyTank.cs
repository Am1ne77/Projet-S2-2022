using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    
    public void Spawn(GameObject bulletGameObject, Vector3 pos, Quaternion rot)
    {
        Instantiate(bulletGameObject, pos, rot);
    }
    
    void Update()
    {
        
    }
}
