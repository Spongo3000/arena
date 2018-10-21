using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NameManager : MonoBehaviour {

    public InputField PlayerNameInputField;


    private void Start()
    {
        string PlayerName = PlayerPrefs.GetString("PlayerName", "");
        if (PlayerName != "")
        {
            PlayerNameInputField.text = PlayerName;
        }
    }

    public void SetPlayerName()
    {
        if (PlayerNameInputField.text == "")
        {
            return;
        }
        PlayerNetwork.Instance.SetPlayerName(PlayerNameInputField.text);
        PlayerPrefs.SetString("PlayerName", PlayerNameInputField.text);
    }

	public void LoadMainMenu()
    {
        SceneManager.LoadScene(GameSettings.MainMenuScene);
    }
}
