using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTarget : MonoBehaviour {

    public PlayerController Player { get; private set; }


    private void Start()
    {
        Player = GetComponentInParent<PlayerController>();
        if (Player.PhotonView.isMine)
        {
            gameObject.layer = 12;
        }
    }
}
