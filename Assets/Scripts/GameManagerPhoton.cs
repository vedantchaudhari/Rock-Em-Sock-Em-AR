using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManagerPhoton : Photon.Pun.MonoBehaviourPunCallbacks
{
    #region Photon Messages

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEntered() IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEntered() IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            LoadArena();
        }
    }

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("PhotonNetwork Error: Attempting to load level, but client is not master client");
            return;
        }

        PhotonNetwork.LoadLevel(0); // Load base arena
    }

    #endregion
}
