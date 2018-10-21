using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {

    [SerializeField]
    private Text _roomName;
    private Text RoomName
    {
        get { return _roomName; }
    }

    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };

        if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
        {

        }
        else
        {
            Debug.LogWarning("Room creation request failed to send.");
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        Debug.Log("Room creation failed: " + codeAndMessage[1].ToString());
    }

    private void OnCreatedRoom()
    {
    }
}
