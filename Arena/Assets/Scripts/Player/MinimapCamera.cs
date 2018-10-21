using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private PlayerController Player;


    private void Start()
    {
        Player = GetComponentInParent<PlayerController>();

        if (!Player.PhotonView.isMine)
        {
            gameObject.SetActive(false);
        }
    }
}
