using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {

    private PhotonView PhotonView;

    public static TeamManager Instance;

    public int Players_TeamRed;
    public int Players_TeamBlue;

    public Transform Spawnpoint_TeamRed;
    public Transform Spawnpoint_TeamBlue;

    public Material Material_TeamRed;
    public Material Material_TeamBlue;

    private void Awake()
    {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();
    }

    public Transform GetSpawnpointForTeam(Team team)
    {
        if (team == Team.Red)
        {
            return Spawnpoint_TeamRed;
        }
        else
        {
            return Spawnpoint_TeamBlue;
        }
    }
    public Material GetMaterialForTeam(Team team)
    {
        if (team == Team.Red)
        {
            return Material_TeamRed;
        }
        else
        {
            return Material_TeamBlue;
        }
    }

}
