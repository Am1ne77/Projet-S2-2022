using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Data;
using Photon.Pun;
using Photon.Realtime;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

using UnityEngine.SceneManagement;

namespace Matchmaking
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public FirebaseManager Firebase;
        [SerializeField] public GameObject OptionsUI;
        public Slider son;

        #region Private Serializable Fields

        [Tooltip(
            "The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")] [SerializeField]
        private GameObject controlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")] [SerializeField]
        private GameObject progressLabel;

        [SerializeField] private ToggleGroup toggleGroup;
        
        private int nbofplayer=2;

        #endregion


        #region Private Fields


        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";


        #endregion


        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            //this.toggleGroup = GetComponent<ToggleGroup>();
            Firebase = FirebaseManager.Instance;
            if (Firebase == null) SceneManager.LoadScene(0);

            PhotonNetwork.AutomaticallySyncScene = true;
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

        }


        private bool switchingscene;

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= nbofplayer && !switchingscene)
            {
                /* for (int i =0; i<PhotonNetwork.PlayerList.Length;i++)
                 {
                     var a= PhotonNetwork.PlayerList[i];
                     for (int j=i+1;j<PhotonNetwork.PlayerList.Length;j++)
                         if (PhotonNetwork.PlayerList[j].NickName==a.NickName)
                         {
                             PhotonNetwork.Disconnect();
                             SceneManager.LoadScene(1);
                             return;
                         }
                     
                 }*/
                switchingscene = true;
                PhotonNetwork.LoadLevel(3);
            }
        }

        #endregion


        #region Public Methods


        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            
            Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
            if (toggle.GetComponentInChildren<Text>().text == "One Player")
            {
                nbofplayer = 1;
            }
            if (PhotonNetwork.LocalPlayer.NickName == null || PhotonNetwork.LocalPlayer.NickName.Length == 0)
            {
                return;
            }

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();


            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
            /*for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                var a = PhotonNetwork.PlayerList[i];
                for (int j = i + 1; j < PhotonNetwork.PlayerList.Length; j++)
                    if (PhotonNetwork.PlayerList[j].NickName == a.NickName)
                    {
                        Debug.Log("same prs");
                        PhotonNetwork.LeaveRoom();
                        PhotonNetwork.LoadLevel(1);
                        return;
                    }

            }*/

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
        }


        #endregion


        #region MonoBehaviourPunCallbacks Callbacks


        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnConnectedToMaster()
        {


            Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");
            PhotonNetwork.JoinRandomRoom();
            /* if (PhotonNetwork.JoinRandomRoom())
             {
                 for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                 {
                     var a = PhotonNetwork.PlayerList[i];
                     for (int j = i + 1; j < PhotonNetwork.PlayerList.Length; j++)
                         if (PhotonNetwork.PlayerList[j].NickName == a.NickName)
                         {
                             Debug.Log("same prs");
                             PhotonNetwork.LeaveRoom();
                             SceneManager.LoadScene(1);
                             return;
                         }
 
                 }
             }*/
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }


        #endregion

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(
                "Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }


        public void SettingScren()
        {
            OptionsUI.SetActive(true);

        }

        public void SettingExit()
        {
            OptionsUI.SetActive(false);
        }

        public void Ondisco()
        {
            PhotonNetwork.LeaveRoom();
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
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

        public void SliderControll()
        {
            Debug.Log(Firebase.Audio);
            Firebase.Audio.volume = son.value;
        }
    }
}
