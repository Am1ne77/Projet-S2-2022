using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.SceneManagement;

public class btn_choose_game : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables

    public void Hangman()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.PlayerList.Length>1)
            {
                return;
            }
        }

        PhotonNetwork.LoadLevel(4);
    }
    public void Puissance_4()
    {

        if (PhotonNetwork.IsConnected )
        {
            if (PhotonNetwork.PlayerList.Length>1 &&!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.LoadLevel(5);
        }
        else SceneManager.LoadScene(5);
    }
    public void MiniTank()
    {
        if (PhotonNetwork.IsConnected)
        { 
            if (PhotonNetwork.PlayerList.Length>1 && !PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.LoadLevel(5);
        }
        else SceneManager.LoadScene(5);
    }
    
    
}
