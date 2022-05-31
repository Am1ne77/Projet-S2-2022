using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using Random = System.Random;

public class Tank : MonoBehaviour
{
    [SerializeField] 
    private GameObject bullet;

    [SerializeField] 
    private GameObject bulletManager;
    
    [SerializeField] 
    private GameObject StartWall1;

    [SerializeField] 
    private GameObject StartWall2;
    
    [SerializeField] 
    private GameObject EnemyTankModel;
    
    [SerializeField] 
    private GameObject EnemyTankManager;

    private DateTime LastShot;

    private bool goingforward;

    #region Spawns
    

    private Vector3[] _spawnpoints = new[]
    {
        new Vector3(104, 2.75f, 35.5f),
        new Vector3(80, 2.75f, 35.5f),
        new Vector3(71.5f, 2.75f, 68),
        new Vector3(74, 2.75f, 103),
        new Vector3(97.5f, 2.75f, 103),
        new Vector3(120, 2.75f, 103),
        new Vector3(123, 2.75f, 68.5f)
    };

    private Quaternion[] _spawnQuaternions = new[]
    {
        new Quaternion(0, 0, 0, 0),
        new Quaternion(0, 0, 0, 0),
        new Quaternion(0, 45, 0, 45),
        new Quaternion(0, 180, 0, 0),
        new Quaternion(0, 180, 0, 0),
        new Quaternion(0, 180, 0, 0),
        new Quaternion(0, 45, 0, -45)
    };
    
    #endregion

    private int _currnbenn = 0;

    void Start()
    {
        //Shooting timer
        LastShot = DateTime.Now;
        
        //collision fix
        goingforward = false;
        
        //start
        StartWall1.SetActive(false);
        StartWall2.SetActive(false);
        
        //spawn first 3 ennemmies
        int nbspawns = _spawnpoints.Length;
        var r = new Random();
        int index1 = r.Next(0, nbspawns);
        int index2 = r.Next(0, nbspawns);
        while (index1 == index2)
        {
            index2 = r.Next(0, nbspawns);
        }
        int index3 = r.Next(0, nbspawns);
        while (index3 == index1 || index3 == index2)
        {
            index3 = r.Next(0, nbspawns);
        }

        var tank1 = EnemyTankManager.AddComponent<EnemyTank>();
        var tank2 = EnemyTankManager.AddComponent<EnemyTank>();
        var tank3 = EnemyTankManager.AddComponent<EnemyTank>();
        
        tank1.Spawn(EnemyTankModel, _spawnpoints[index1], _spawnQuaternions[index1]);
        tank2.Spawn(EnemyTankModel, _spawnpoints[index2], _spawnQuaternions[index2]);
        tank3.Spawn(EnemyTankModel, _spawnpoints[index3], _spawnQuaternions[index3]);
        
        _currnbenn = 3;
    }

    void Update()
    {
        if (_currnbenn < 3)
        {
            var r = new Random();
            int index = r.Next(0, _spawnpoints.Length);
            var tank = EnemyTankManager.AddComponent<EnemyTank>();
            tank.Spawn(EnemyTankModel, _spawnpoints[index], _spawnQuaternions[index]);
            _currnbenn++;
        }
        
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0,0,(float) 0.2));
            goingforward = true;
        }
        
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0,0,(float) -0.2));
            goingforward = false;
        }
        
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0,-4,0), Space.Self);
        }
        
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0,4,0), Space.Self);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TimeSpan ready = DateTime.Now - LastShot;
            if (ready.Seconds >= 3)
            {
                var bull = bulletManager.AddComponent<Bullet>();
                bull.Shoot(bullet, transform.position, transform.rotation);
                LastShot = DateTime.Now;
                Destroy(bulletManager.GetComponent<Bullet>());
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Walls"))
        {
            if (goingforward)
            {
                transform.Translate(new Vector3(0, 0, -1));
            }
            else
            {
                transform.Translate(new Vector3(0, 0, 1));
            }
        }

    }
}
