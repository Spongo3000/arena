using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairHitmarker : MonoBehaviour {

    private PlayerController Player;
    private PhotonView PhotonView;
    private Image Crosshair;


    private void Start()
    {
        Player = GetComponentInParent<PlayerController>();
        PhotonView = Player.PhotonView;
        Crosshair = GetComponent<Image>();

        Crosshair.color = Color.green;

        if (!PhotonView.isMine)
        {
            enabled = false;
        }
    }

    public void HitPlayer()
    {
        StartCoroutine(IE_HitPlayer());
    }

    public IEnumerator IE_HitPlayer()
    {
        Crosshair.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        Crosshair.color = Color.green;
    }

    private void OnDisable()
    {
        if (Crosshair != null)
        {
            Crosshair.color = Color.green;
        }
    }
}
