using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour {

    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; }
    private PhotonView PhotonView;
    private int PlayersInGame = 0;

    public GameMode gameMode = GameMode.OneVersusOne;

	private void Awake ()
    {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();

        PlayerName = "Player" + Random.Range(1, 1000).ToString();

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == GameSettings.GameScene)
        {
            if (PhotonNetwork.isMasterClient)
            {
                MasterLoadedGame();
            }
            else
            {
                NonMasterLoadedGame();
            }
        }
        else if (scene.buildIndex == GameSettings.FreeForAllScene)
        {
            if (PhotonNetwork.isMasterClient)
            {
                MasterLoadedGame();
            }
            else
            {
                NonMasterLoadedGame();
            }
        }
    }

    private void MasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }

    private void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        //1V1
        if (gameMode == GameMode.OneVersusOne)
        {
            PhotonNetwork.LoadLevel(GameSettings.GameScene);
        }
        //FFA
        else if (gameMode == GameMode.FreeForAll)
        {
            PhotonNetwork.LoadLevel(GameSettings.FreeForAllScene);
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        PlayersInGame++;
        if (PlayersInGame == PhotonNetwork.playerList.Length)
        {
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        if (gameMode == GameMode.OneVersusOne)
        {
            Team NextPlayersTeam;

            if (PlayersInGame == 0)
            {
                NextPlayersTeam = Team.Blue;
            }
            else
            {
                NextPlayersTeam = Team.Red;
            }

            //Team NextPlayersTeam = TeamManager.Instance.GetLowerTeam();
            //TeamManager.Instance.IncreaseTeam(NextPlayersTeam);

            Transform t = TeamManager.Instance.GetSpawnpointForTeam(NextPlayersTeam);

            GameObject player = PhotonNetwork.Instantiate("Arena_Player", t.position, t.rotation, 0);
            player.GetComponent<TeamMember>().SetTeam(NextPlayersTeam);
        }
        else if (gameMode == GameMode.FreeForAll)
        {
            Transform spawnPoint = MapManager.Instance.GetRandomSpawnpoint();

            PhotonNetwork.Instantiate("Arena_Player", spawnPoint.position, spawnPoint.rotation, 0);
        }
    }

    [PunRPC]
    private void RPC_ToggleGameMode()
    {
        if (gameMode == GameMode.OneVersusOne)
        {
            gameMode = GameMode.FreeForAll;
        }
        else if (gameMode == GameMode.FreeForAll)
        {
            gameMode = GameMode.OneVersusOne;
        }
    }

    public void ToggleGameMode()
    {
        PhotonView.RPC("RPC_ToggleGameMode", PhotonTargets.All);
    }
}
public enum GameMode { OneVersusOne, FreeForAll }