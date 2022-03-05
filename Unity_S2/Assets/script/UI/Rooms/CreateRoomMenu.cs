 using System.Collections;
using System.Collections.Generic;
 using Photon.Pun;
 using UnityEngine;
 using UnityEngine.UI;
 using Photon.Realtime;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] 
    private Text _roomName;

    private RoomsCanvases _roomCanvases;
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomCanvases = canvases;
    }
    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.InRoom) //on a rajouter .InRoom pour eviter l'erreur car il est
        {
            return;                                             //deja dans une room
        }
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.PublishUserId = true;
        options.BroadcastPropsChangeToAll = true;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room successfully",this);
        _roomCanvases.CurrentRoomCanvas.Show();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed: " + message);
    }
}
