using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Connecting to Photon ...", this);
        AuthenticationValues authValues = new AuthenticationValues("0");
        PhotonNetwork.AuthValues = authValues;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 5;
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon" , this);
        Debug.Log("My nickname is " + PhotonNetwork.LocalPlayer.NickName , this);
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server for reason "+cause.ToString());
        
    }

    public override void OnJoinedLobby()
    {
        print("joined lobby");
        //PhotonNetwork.FindFriends(new string[]{"1"});
    }

    /*public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        base.OnFriendListUpdate(friendList);

        foreach (var friendInfo in friendList)
        {
            Debug.Log("Friend info received " + friendInfo.UserId + " is online " + friendInfo.IsOnline);
        }
    }*/
    
}
