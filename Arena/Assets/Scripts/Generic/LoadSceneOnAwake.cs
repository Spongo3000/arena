using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnAwake : MonoBehaviour {

    private void Awake()
    {
        SceneManager.LoadScene(GameSettings.MainMenuScene);
    }
}
