using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviour
{
    #region Public Variables

    #endregion

    #region Private Variables

    private string _version = "1.0";

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

    #region Public Methods

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #endregion
}