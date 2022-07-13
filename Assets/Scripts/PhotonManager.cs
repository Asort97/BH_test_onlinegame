using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text nickField;
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void CreateRoom(Text inputField)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;
        PhotonNetwork.CreateRoom(inputField.text, roomOptions);
    }
    public void ConnectRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Gameplay");
    }
}
