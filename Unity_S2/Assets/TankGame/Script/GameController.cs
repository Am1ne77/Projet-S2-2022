using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using System;
using System.Xml.Serialization;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Data;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


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

    private Vector3 PlayerSpawnLocation = new Vector3(100, 2.75f, 70);

    private Quaternion PlayerSpawnRotation = Quaternion.Euler(0, 0, 0);
    
    #endregion
    
    private static int _currnbenemy = 0;
    public static FirebaseManager Firebase;
    public static DateTime start;

    public static bool isEnd = false;
    
    public  Text score;
    public  GameObject endGameUI;
    public GameObject settings;
    public  AudioSource victorySound;
    
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


    private void DetermineSpawnForScene()
    {
        start = DateTime.Now;
        if (SceneManager.GetActiveScene().name == "TankLvl2")
        {
            PlayerSpawnLocation = new Vector3(120, 1.5f, 88);
            PlayerSpawnRotation = Quaternion.Euler(0, 180, 0);
            
            _spawnpoints = new[]
            {
                new Vector3(94,1.5f,83),
                new Vector3(94,1.5f,99),
                new Vector3(119,1.5f,104),
                new Vector3(147,1.5f,97.6f),
                new Vector3(147,1.5f,78),
                new Vector3(131.5f,1.5f,73),
                new Vector3(106,1.5f,73),
            };

            _spawnQuaternions = new[]
            {
                Quaternion.Euler(0, 90, 0),
                Quaternion.Euler(0, 90, 0),
                Quaternion.Euler(0, 180, 0),
                Quaternion.Euler(0, -90, 0),
                Quaternion.Euler(0, -90, 0),
                Quaternion.Euler(0, 0, 0),
                Quaternion.Euler(0, 0, 0),
            };
        }
    }
    
    void Start()
    {
        Firebase=FirebaseManager.Instance;
        Firebase.Audio.Pause();

        //Determine the spawn location depending on the scene
        DetermineSpawnForScene();

        //spawn player
        var player = Instantiate(PlayerModel, PlayerSpawnLocation, PlayerSpawnRotation);
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

        if (isEnd)
        {
            isEnd = false;
            Invoke("EndGame",1);
        }
            
        
    }
    public void EndGame()
    {
        settings.SetActive(false);
        DateTime time = DateTime.Now;
        TimeSpan interval =  time- start;
        int xp = (int) (interval.TotalMilliseconds) / 30;
        Firebase.xpMiniTank += xp;
        Firebase.SaveDataButton("MiniTank");
        //PhotonNetwork.LoadLevel(3);
        score.text = xp.ToString();
        endGameUI.SetActive(true);
        victorySound.Play();
    }

    public static void EnemyDestroyed()
    {
        Firebase.xpMiniTank += 100;
        //Updates the current number of enemies alive
        _currnbenemy--;
    }
}
