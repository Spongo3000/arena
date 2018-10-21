using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour {

    public static MainCanvasManager Instance;

    [SerializeField]
    private LobbyCanvas _lobbyCanvas;
    public LobbyCanvas LobbyCanvas
    {
        get { return _lobbyCanvas; }
    }

    [SerializeField]
    private CurrentRoomCanvas _currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas
    {
        get { return _currentRoomCanvas; }
    }

    [SerializeField]
    private GameObject _mainMenu;
    public GameObject MainMenu
    {
        get { return _mainMenu; }
    }

    [SerializeField]
    private GameObject _loadoutMenu;
    public GameObject LoadoutMenu
    {
        get { return _loadoutMenu; }
    }


    private void Awake()
    {
        Instance = this;
    }

    public void OnClick_OpenLobbyMenu()
    {
        LobbyCanvas.transform.SetAsLastSibling();
    }

    public void OnClick_OpenLoadoutMenu()
    {
        LoadoutMenu.transform.SetAsLastSibling();
    }

    public void OnClick_OpenMainMenu()
    {
        MainMenu.transform.SetAsLastSibling();
    }
}
