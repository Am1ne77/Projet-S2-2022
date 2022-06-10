using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Photon.Pun;
using UnityEngine;
using Firebase;
using Firebase.Auth;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu1_script : MonoBehaviour
{
    public GameObject OptionsUI;
    public GameObject MenuUI;
    
    public static FirebaseManager Firebase;
    public Slider son;
    
    [SerializeField] 
    private AudioSource Macron;
    
    
    private void Start()
    {
        Firebase = FirebaseManager.Instance;
        
        if (!Firebase.Audio.isPlaying) Firebase.Audio.Play();
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
        MenuUI.SetActive(false);
    } 
    
    public void SettingExit()
    {
        OptionsUI.SetActive(false);
        
        MenuUI.SetActive(true);
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
        Firebase.SignOutButton();
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
