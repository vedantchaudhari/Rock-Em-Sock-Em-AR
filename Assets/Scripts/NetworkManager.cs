using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Public Variables

    #endregion

    #region Private Variables

    private string _version = "1.0";
    private byte maxPlayers = 2;

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        // Lets us use PhotoNetwork.LoadLevel on master client and all clients sync their levels uatomatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Connect();
    }

    #endregion

    #region MonoBehaviour PUN Callbacks

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to master server");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogWarningFormat("NetworkManager: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("NetworkManager: OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("NetworkManager: OnJoinedRoom() called by PUN. This client is now connected to a room");
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = _version;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #endregion
}