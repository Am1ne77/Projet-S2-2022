using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using System;

public class GameController : MonoBehaviour
{
    
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
    
    private static int _currnbenemy = 0;
    
    [SerializeField] 
    private GameObject EnemyTankModel;

    [SerializeField] 
    private GameObject PlayerModel;
    
    [SerializeField] 
    private GameObject BulletModel;

    [SerializeField] 
    private AudioSource ShootSnd;
    
    [SerializeField] 
    private AudioSource EmptyMag;
    
    [SerializeField] 
    private AudioSource Killed;
    
    
    void Start()
    {
        //spawn player
        var player = Instantiate(PlayerModel, new Vector3(100, 2.75f, 70), Quaternion.Euler(0, 0, 0));
        player.GetComponent<Tank>().bullet = BulletModel;
        player.GetComponent<Tank>().shootsnd = ShootSnd;
        player.GetComponent<Tank>().emptyMag = EmptyMag;
        player.GetComponent<Tank>().killed = Killed;
        
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
        
        
        var tank1 = Instantiate(EnemyTankModel, _spawnpoints[index1], _spawnQuaternions[index1]);
        tank1.GetComponent<EnemyTank>().bullet = BulletModel;
        tank1.GetComponent<EnemyTank>().shootsnd = ShootSnd;
        tank1.GetComponent<EnemyTank>().killed = Killed;
        
        var tank2 = Instantiate(EnemyTankModel, _spawnpoints[index2], _spawnQuaternions[index2]);
        tank2.GetComponent<EnemyTank>().bullet = BulletModel;
        tank2.GetComponent<EnemyTank>().shootsnd = ShootSnd;
        tank2.GetComponent<EnemyTank>().killed = Killed;
        
        var tank3 = Instantiate(EnemyTankModel, _spawnpoints[index3], _spawnQuaternions[index3]);
        tank3.GetComponent<EnemyTank>().bullet = BulletModel;
        tank3.GetComponent<EnemyTank>().shootsnd = ShootSnd;
        tank3.GetComponent<EnemyTank>().killed = Killed;

        _currnbenemy = 3;
    }
    
    void FixedUpdate()
    {
        //Respawn enemy
        if (_currnbenemy < 3)
        {
            var r = new Random();
            int index1 = r.Next(0, _spawnpoints.Length);
            var tank1 = Instantiate(EnemyTankModel, _spawnpoints[index1], _spawnQuaternions[index1]);
            tank1.GetComponent<EnemyTank>().bullet = BulletModel;
            tank1.GetComponent<EnemyTank>().shootsnd = ShootSnd;
            tank1.GetComponent<EnemyTank>().killed = Killed;
            _currnbenemy++;
        }
    }

    public static void EndGame()
    {
        
    }

    public static void EnemyDestroyed()
    {
        //Updates the current number of enemies alive
        _currnbenemy--;
    }
}
