using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject bullet;
    public void Shoot(GameObject bulletGameObject, Vector3 pos, Quaternion rot)
    {
        var bul = Instantiate(bulletGameObject, pos, rot);
        this.bullet = bul;
    }
    
    void Start()
    {
        
    }

    
    void Update()
    {
        bullet.transform.Translate(new Vector3(0,0,1));
    }
}
