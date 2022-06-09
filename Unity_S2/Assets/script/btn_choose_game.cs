using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;


using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class btn_choose_game : MonoBehaviour
{
    public static UIManager instance;
    public static FirebaseManager Firebase;
    
    public GameObject OptionsUI;
    public GameObject hangManButton;
    public GameObject tankButton;

    public Slider slider;

    //Screen object variables
    public void Start()
    {
        Firebase=FirebaseManager.Instance;
        
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            hangManButton.gameObject.SetActive(false);
            tankButton.gameObject.SetActive(false);
        }
    }

    public void Hangman()
    {
        PhotonNetwork.LoadLevel(4);
    }
    public void Puissance_4()
    {
        PhotonNetwork.LoadLevel(5); 
        //else SceneManager.LoadScene(5);
    }
    public void MiniTank()
    {
        Random r = new Random();
        int lev = r.Next(6,8);
        
        PhotonNetwork.LoadLevel(lev);
       // else SceneManager.LoadScene(6);
    }
    
    public void SettingScren()
    {
        OptionsUI.SetActive(true);
    }

    public void SettingExit()
    {
        OptionsUI.SetActive(false);
    }
    
    public void MainMenu()
    {
        Firebase.SignOutButton();
        PhotonNetwork.DestroyAll();
        SceneManager.LoadScene(1);

    }
    
    public void SliderControll()
    {
        Firebase.Audio.volume = slider.value;
    }



}
