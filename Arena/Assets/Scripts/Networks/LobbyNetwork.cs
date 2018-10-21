using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour {

    private void Start()
    {
        //Version is currently 0.0.1
        PhotonNetwork.ConnectUsingSettings(GameSettings.Version);
    }

    private void OnConnectedToMaster()
    {

        if (PlayerNetwork.Instance == null)
        {
            Debug.LogWarning("Change back to the setup scene, before you start, you idiot.");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.playerName = PlayerNetwork.Instance.PlayerName;

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    private void OnJoinedLobby()
    {
        if (!PhotonNetwork.inRoom)
        {
            //MainCanvasManager.Instance.LobbyCanvas.transform.SetAsLastSibling();
        }
    }
}
