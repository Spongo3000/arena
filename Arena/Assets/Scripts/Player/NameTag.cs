using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTag : MonoBehaviour {

    private PlayerController Player;


    private void Start()
    {
        Player = GetComponentInParent<PlayerController>();

        if (Player.PhotonView.isMine)
        {
            gameObject.SetActive(false);
        }
    }

    public void LookAtMe(Camera cam)
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
