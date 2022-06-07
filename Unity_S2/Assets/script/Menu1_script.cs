using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Photon.Pun;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Data;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu1_script : MonoBehaviour
{
    public GameObject OptionsUI;
    public static FirebaseManager Firebase;
    public Slider son;
    
    [SerializeField] 
    private AudioSource Macron;
    
    
    private void Start()
    {
        Firebase = FirebaseManager.Instance;
    }
     
    private void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void SettingScren()
    {
        OptionsUI.SetActive(true);
    } 
    
    public void SettingExit()
    {
        OptionsUI.SetActive(false);
    } 
    
    public void SettingDisco()
    {
        Firebase.SignOutButton();
        GameObject.Destroy(FirebaseManager.Instance);
        Destroy(Firebase.Audio);
        Destroy(Firebase);
        Debug.Log(Firebase);
        SceneManager.LoadScene(0);

    } 
    public void CloseApp()
    {
        Application.Quit();
    }

    public void ChangeMenu()
    {
		SceneManager.LoadScene(2);
    }

    public void Credits()
    {
        Macron.Play();
    }
    public void SliderControll()
    {
        Debug.Log(Firebase.Audio);
        Firebase.Audio.volume = son.value;
    }
}
