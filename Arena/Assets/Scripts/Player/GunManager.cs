using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour {

    public PlayerController Player { get; private set; }

    public int selectedWeapon = 0;
    public Gun CurrentGun { get { return transform.GetChild(selectedWeapon).GetComponent<Gun>(); } }


	private void Start ()
    {
        Player = GetComponentInParent<PlayerController>();
        SelectWeapon(Player.loadout.PrimaryWeaponName);
	}
	
	private void Update ()
    {
        //Only control locally
        if (!Player.PhotonView.isMine)
        {
            return;
        }

        //Different ways of selecting
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (transform.GetChild(selectedWeapon).name == Player.loadout.PrimaryWeaponName)
            {
                SelectWeapon(Player.loadout.SecondaryWeaponName);
            }
            else if (transform.GetChild(selectedWeapon).name == Player.loadout.SecondaryWeaponName)
            {
                SelectWeapon(Player.loadout.PrimaryWeaponName);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (transform.GetChild(selectedWeapon).name == Player.loadout.PrimaryWeaponName)
            {
                SelectWeapon(Player.loadout.SecondaryWeaponName);
            }
            else if (transform.GetChild(selectedWeapon).name == Player.loadout.SecondaryWeaponName)
            {
                SelectWeapon(Player.loadout.PrimaryWeaponName);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && transform.childCount >= 1)
        {
            SelectWeapon(Player.loadout.PrimaryWeaponName);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            SelectWeapon(Player.loadout.SecondaryWeaponName);
        }
    }

    public void SelectWeapon(string weaponName)
    {
        int i = 0;
        foreach (Transform gun in transform)
        {
            if (transform.GetChild(i).name == weaponName)
            {
                gun.gameObject.SetActive(true);
                foreach (Transform gunParts in gun.transform)
                {
                    gunParts.gameObject.SetActive(true);
                }
                selectedWeapon = i;
            }
            else
            {
                gun.gameObject.SetActive(false);
                foreach (Transform gunParts in gun.transform)
                {
                    gunParts.gameObject.SetActive(false);
                }
            }
            i++;
        }
    }

    public void ReloadAllGuns()
    {
        foreach (Transform weapon in transform)
        {
            Gun gun = weapon.GetComponent<Gun>();
            gun.ammo = gun.maxAmmo;
        }
    }
}
