using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu1_script : MonoBehaviour
{
    private void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void ChangeMenu()
    {
        SceneManager.LoadScene(1);
    }
}
