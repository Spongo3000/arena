using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerManager : MonoBehaviour {

    public static PlayerManager Instance;
    private PhotonView PhotonView;

    public float RestartTime = 5f;
    public GameObject DeathCamera;


    private void Awake()
    {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();
    }

    public void Respawn(PlayerController player)
    {
        StartCoroutine(RespawnPlayer(player));
    }

    private IEnumerator RespawnPlayer(PlayerController player)
    {
        player.Dead = true;
        foreach (GameObject mesh in player.Meshes)
        {
            mesh.SetActive(false);
        }
        for (int i = 0; i < player.gunManager.transform.childCount; i++)
        {
            player.gunManager.transform.GetChild(i).gameObject.SetActive(false);
        }
        player.DeathCamera.SetActive(true);
        player.cam.SetActive(false);

        ScoreboardManager.Instance.ShowScoreboard();

        //-----------------------------------------------

        yield return new WaitForSeconds(RestartTime);
        
        //-----------------------------------------------

        if (PlayerNetwork.Instance.gameMode == GameMode.OneVersusOne)
        {
            Transform teamPos = TeamManager.Instance.GetSpawnpointForTeam(player.GetComponent<TeamMember>().Team);
            player.transform.position = teamPos.position;
            player.transform.rotation = teamPos.rotation;
            player.cam.transform.rotation = new Quaternion(teamPos.rotation.x, player.cam.transform.rotation.y, player.cam.transform.rotation.z, player.cam.transform.rotation.w);
        }
        else if (PlayerNetwork.Instance.gameMode == GameMode.FreeForAll)
        {
            Transform t = MapManager.Instance.GetRandomSpawnpoint();
            player.transform.position = t.position;
            player.transform.rotation = t.rotation;
            player.cam.transform.rotation = new Quaternion(t.rotation.x, player.cam.transform.rotation.y, player.cam.transform.rotation.z, player.cam.transform.rotation.w);
        }
        for (int i = 0; i < player.gunManager.transform.childCount; i++)
        {
            player.gunManager.transform.GetChild(i).gameObject.SetActive(false);
        }
        player.gunManager.transform.GetChild(player.gunManager.selectedWeapon).gameObject.SetActive(true);

        player.Health = 100f;
        player.gunManager.ReloadAllGuns();

        player.DeathCamera.SetActive(false);
        player.cam.SetActive(true);
        foreach (GameObject mesh in player.Meshes)
        {
            mesh.SetActive(true);
        }

        ScoreboardManager.Instance.HideScoreboard();

        player.Dead = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.collider.name);
    }
}
