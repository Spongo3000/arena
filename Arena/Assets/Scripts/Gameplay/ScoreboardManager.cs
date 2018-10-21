using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour {

    public static ScoreboardManager Instance;

    private PhotonView PhotonView;
    public GameObject ScoreboardLayoutGroup;
    public GameObject ScoreboardElementPrefab;

    private List<ScoreboardObject> PlayersInScoreboard = new List<ScoreboardObject>();


	private void Awake() {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();
	}

    [PunRPC]
    private void RPC_IncreaseKills(string playerName)
    {
        ScoreboardObject element = GetPlayerOrCreateNew(playerName);
        element.Kills++;
        element.Text.text = element.PlayerName + ": " + element.Kills.ToString() + "/" + element.Deaths.ToString();
    }

    public void IncreaseKills(string playerName)
    {
        PhotonView.RPC("RPC_IncreaseKills", PhotonTargets.All, playerName);
    }

    [PunRPC]
    private void RPC_IncreaseDeaths(string playerName)
    {
        ScoreboardObject element = GetPlayerOrCreateNew(playerName);
        element.Deaths++;
        element.Text.text = element.PlayerName + ": " + element.Kills.ToString() + "/" + element.Deaths.ToString();
    }

    public void IncreaseDeaths(string playerName)
    {
        PhotonView.RPC("RPC_IncreaseDeaths", PhotonTargets.All, playerName);
    }

    private ScoreboardObject GetPlayerOrCreateNew(string PlayerName)
    {
        int index = PlayersInScoreboard.FindIndex(x => x.PlayerName == PlayerName);

        if (index == -1)
        {
            GameObject element = Instantiate(ScoreboardElementPrefab, ScoreboardLayoutGroup.transform);

            Text ElementText = element.GetComponent<Text>();

            ScoreboardObject newScoreboardElement = new ScoreboardObject(ElementText, PlayerName);

            PlayersInScoreboard.Add(newScoreboardElement);
            return newScoreboardElement;
        }
        else
        {
            return PlayersInScoreboard[index];
        }
    }

    public void ShowScoreboard()
    {
        ScoreboardLayoutGroup.SetActive(true);
    }

    public void HideScoreboard()
    {
        ScoreboardLayoutGroup.SetActive(false);
    }
}
public class ScoreboardObject
{
    public Text Text;
    public string PlayerName;
    public int Kills = 0;
    public int Deaths = 0;

    public ScoreboardObject(Text text, string playerName)
    {
        Text = text;
        PlayerName = playerName;
    } 

    public ScoreboardObject(Text text, string playerName, int kills, int deaths)
    {
        Text = text;
        PlayerName = playerName;
        Kills = kills;
        Deaths = deaths;
    }
}