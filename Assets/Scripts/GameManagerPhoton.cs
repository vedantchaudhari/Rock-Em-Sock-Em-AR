﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManagerPhoton : MonoBehaviourPunCallbacks
{
    public static GameManagerPhoton Instance;
    public GameObject redRobot;
    public GameObject blueRobot;
    public GameObject[] Spawns;

    #region Photon Messages

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.LogFormat("GameManager: OnPlayerEnteredRoom() {0}", newPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEntered() IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.LogFormat("GameManager: OnPlayerLeftRoom() {0}", otherPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("GameManager: OnPlayerEntered() IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            LoadArena();
        }
    }

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        Instance = this;
        
        Debug.LogAssertionFormat("Local Player is MasterClient: {0}", PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient && NetworkedPlayerControllerPhoton.localPlayerInstance == null)
        {
            PhotonNetwork.Instantiate(redRobot.name, Spawns[0].transform.position, Quaternion.identity, 0);
        }
        else if (NetworkedPlayerControllerPhoton.localPlayerInstance == null)
        {
            PhotonNetwork.Instantiate(blueRobot.name, Spawns[1].transform.position, Quaternion.identity * Quaternion.Euler(0f, 180f, 0f), 0);
        }
    }

    private void Awake()
    {
        
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Private Methods

    private void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("PhotonNetwork Error: Attempting to load level, but client is not master client");
            return;
        }

        PhotonNetwork.LoadLevel(1); // Load base arena
    }

    #endregion
}
