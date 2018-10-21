using UnityEngine;

public class TeamMember : MonoBehaviour {

    public Team Team { get; private set; }


    public void SetTeam(Team team)
    {
        Team = team;
    }
}
