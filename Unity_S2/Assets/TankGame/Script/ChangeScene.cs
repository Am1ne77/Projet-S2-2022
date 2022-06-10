using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class ChangeScene : MonoBehaviour
{
    public FirebaseManager Firebase;
    
    public GameObject OptionsUI;

    public Slider slider;
    private void Start()
    {
        Firebase=FirebaseManager.Instance;
    }

    public void SettingScren()
    {
        OptionsUI.SetActive(true);
    }

    public void SettingExit()
    {
        OptionsUI.SetActive(false);
    }
    public void Replay()
    {
        Random r = new Random();
        int lev = r.Next(6,8);
        
        PhotonNetwork.LoadLevel(lev);
    }

    public void ChangeGame()
    {
        PhotonNetwork.LoadLevel(3);
    }

    public void MainMenu()
    {
        PhotonNetwork.LoadLevel(1);
        PhotonNetwork.LeaveRoom();


    }
    
    public void SliderControll()
    {
        Firebase.Audio.volume = slider.value;
    }

}
