using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNametag : MonoBehaviour {

    private TextMesh currentTarget;

    private PlayerController Player;
    private Camera cam;

    public LayerMask raycastLayerMask;


    private void Start()
    {
        Player = GetComponent<PlayerController>();
        cam = Player.cam.GetComponent<Camera>();

        if (!Player.PhotonView.isMine)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            currentTarget.gameObject.SetActive(false);
            currentTarget = null;
        }
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10000f, raycastLayerMask))
        {
            GunTarget target = hit.collider.GetComponent<GunTarget>();
            if (target != null)
            {
                currentTarget = target.Player.NameplateTextMesh;
            }
        }
        if (currentTarget != null)
        {
            currentTarget.gameObject.SetActive(true);
            currentTarget.GetComponent<NameTag>().LookAtMe(cam);
        }
    }
}
