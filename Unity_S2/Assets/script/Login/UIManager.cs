using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    
    public GameObject loginUI;
    public GameObject registerUI;

    public GameObject OptionsUI;

    [Header("Settings")] 
    
    public GameObject UIprecedent;
    public Button uninstall;

    public Button close;

    public Slider son;
    //public GameObject userDataUI;
        //public GameObject scoreboardUI;

    private void Awake()
    {
        if (instance==null){
            instance=this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
        
       /* if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }*/
    }

    //Functions to change the login screen UI

    public void ClearScreen() //Turn off all screens
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        
    }

    public void LoginScreen() //Back button
    {
        UIprecedent = loginUI;
        
        registerUI.SetActive(false);
        
        loginUI.SetActive(true);
    }
    public void RegisterScreen() // Regester button
    {
        UIprecedent = registerUI;
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }

    public void SettingScreen()
    {
        UIprecedent.SetActive(false);
        OptionsUI.SetActive(true);
    }

    public void SettingExit()
    {
        OptionsUI.SetActive(false);
        UIprecedent.SetActive(true);
    }

   public void UserDataScreen() //Logged in
    {
     ClearScreen();
        //userDataUI.SetActive(true);
    }

    public void ScoreboardScreen() //Scoreboard button
    {
    ClearScreen();
    //scoreboardUI.SetActive(true);
    }

    public void SliderControll()
    {
        var song = FirebaseManager.Instance.Audio;
        song.volume = son.value;
    }
}
