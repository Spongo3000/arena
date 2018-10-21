using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour {

    public void OnClick_StartSync()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        GameMode gameMode = PlayerNetwork.Instance.gameMode;
        if (gameMode == GameMode.OneVersusOne)
        {
            PhotonNetwork.LoadLevel(GameSettings.GameScene);
        }
        else if (gameMode == GameMode.FreeForAll)
        {
            PhotonNetwork.LoadLevel(GameSettings.FreeForAllScene);
        }
    }

    public void OnClick_StartDelayed()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;

        GameMode gameMode = PlayerNetwork.Instance.gameMode;
        if (gameMode == GameMode.OneVersusOne)
        {
            PhotonNetwork.LoadLevel(GameSettings.GameScene);
        }
        else if (gameMode == GameMode.FreeForAll)
        {
            PhotonNetwork.LoadLevel(GameSettings.FreeForAllScene);
        }
    }

}
