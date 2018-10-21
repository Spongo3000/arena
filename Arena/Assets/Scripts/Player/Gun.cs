using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public GunManager gunManager { get; private set; }
    public Animator animator;
    private Scope scope;

    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float reloadTime = 1f;
    public float ammo;
    public float maxAmmo = 30;
    public float recoilModifier = 0.5f;
    public float sprayModifier = 0.5f;
    public float scopedSprayModifier = 0.1f;
    public float scopeTime = 0.15f;
    public float scopeFOV = 15f;
    public LayerMask gunLayerMask;
    public ShootingMode shootingMode = ShootingMode.SemiAutomatic;
    public ScopeMode scopeMode;

    public Camera cam;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public AudioClip sound;

    private float nextTimeToFire = 0f;
    public bool reloading { get; private set; }
    public bool Scoped { get { return scope.scoped; } }


    private void Awake()
    {
        gunManager = GetComponentInParent<GunManager>();
        scope = GetComponent<Scope>();
        ammo = maxAmmo;

        if (!gunManager.Player.PhotonView.isMine)
        {
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        reloading = false;
        animator.SetBool("Reloading", false);
    }

    private void Update()
    {
        if (gunManager.Player.Dead || reloading)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && ammo != maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        if (ammo <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                StartCoroutine(Reload());
            }
            return;
        }

        if (shootingMode == ShootingMode.Automatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else if (shootingMode == ShootingMode.SemiAutomatic)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else if (shootingMode == ShootingMode.Burst)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate * 1;
                StartCoroutine(BurstShoot());
            }
        }
    }

    private IEnumerator BurstShoot()
    {
        for (int i = 0; i < fireRate; i++)
        {
            Shoot();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Reload()
    {
        scope.StopCoroutine("ScopeSniper");
        scope.StopCoroutine("ScopeIronsight");

        scope.UnscopeGun();
        reloading = true;
        animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime - 0.25f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(0.25f);

        ammo = maxAmmo;
        reloading = false;
    }

    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            return;
        }

        ammo--;
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        if (sound != null)
        {
            gunManager.Player.PlaySound(gunManager.selectedWeapon);
        }

        ApplyRecoil();
        RaycastHit hit;

        float r1 = Random.Range(0.01f, 1f);
        float r2 = Random.Range(0.01f, 1f);
        float r3 = Random.Range(0, 2);
        if (r3 == 1)
        {
            r3 = -1;
        }
        else
        {
            r3 = 1;
        }
        float r4 = Random.Range(0, 2);
        if (r4 == 1)
        {
            r4 = -1;
        }
        else
        {
            r4 = 1;
        }

        Vector3 angle = cam.transform.forward;
        angle = new Vector3(angle.x - 0.05f * r3 * Mathf.Log(r1) * sprayModifier, angle.y - 0.05f * r4 * Mathf.Log(r2) * sprayModifier, angle.z);
        if (!gunManager.Player.GetComponent<CharacterController>().isGrounded)
        {
            angle *= 2;
        }

        if (Physics.Raycast(cam.transform.position, angle, out hit, range, gunLayerMask))
        {
            GunTarget target = hit.collider.GetComponent<GunTarget>();

            if (target != null && target.Player != gunManager.Player)
            {
                gunManager.Player.crosshairHitmarker.HitPlayer();
                if (target.gameObject.layer == 10)
                {
                    target.Player.TakeDamage(damage * GameSettings.HeadshotMultiplier, gunManager.Player.NameplateTextMesh.text);
                }
                else if (target.gameObject.layer == 11)
                {
                    target.Player.TakeDamage(damage, gunManager.Player.NameplateTextMesh.text);
                }
            }
            
            //GameObject impactGO = PhotonNetwork.Instantiate("Effect 2", hit.point, Quaternion.LookRotation(hit.normal), 0);
            //StartCoroutine(DestroyImpactGO(impactGO));
        }
    }

    private void ApplyRecoil()
    {
        gunManager.Player.IncreaseVerticalRotation(recoilModifier);
    }

    private IEnumerator DestroyImpactGO(GameObject impactGO)
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.Destroy(impactGO);
    }
}
public enum ShootingMode { SemiAutomatic, Burst, Automatic }