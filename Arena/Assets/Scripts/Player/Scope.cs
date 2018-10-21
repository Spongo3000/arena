using System.Collections;
using UnityEngine;

public class Scope : MonoBehaviour {

    private PlayerController Player;

    public GameObject scopeOverlay;
    public GameObject crosshair;

    private Animator animator;
    private Gun gun;

    [HideInInspector] public bool scoped = false;
    private float previousFOV;
    public float previousSense { get; set; }
    private float previousSprayModifier;

    private Coroutine scopeCoroutine;


    private void Start()
    {
        animator = GetComponent<Animator>();
        gun = GetComponent<Gun>();
        Player = gun.gunManager.Player;
        previousFOV = Player.cam.GetComponent<Camera>().fieldOfView;
        previousSense = Player.RotationSpeed;
        previousSprayModifier = gun.sprayModifier;

        if (!Player.PhotonView.isMine)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (gun.scopeMode == ScopeMode.None)
        {
            return;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (animator.IsInTransition(0) || gun.reloading)
            {
                return;
            }

            scoped = !scoped;
            animator.SetBool("Scoped", scoped);

            if (scoped)
            {
                if (gun.muzzleFlash != null)
                {
                    gun.muzzleFlash.gameObject.SetActive(false);
                }
                ScopeGun();
            }
            else
            {
                UnscopeGun();
            }
        }
    }

    private void ScopeGun()
    {
        Player.MoveSpeed = Player.NormalMoveSpeed * Player.SneakMultiplier;
        if (gun.scopeMode == ScopeMode.Sniper && scoped)
        {
            scopeCoroutine = StartCoroutine(ScopeSniper());
        }
        else if (gun.scopeMode == ScopeMode.Ironsight)
        {
            scopeCoroutine = StartCoroutine(ScopeIronsight());
        }
        else if (gun.scopeMode == ScopeMode.RedDot)
        {
            Debug.Log("RedDot to be added.");
        }
    }

    public void UnscopeGun()
    {
        if (scopeCoroutine != null)
        {
            StopCoroutine(scopeCoroutine);
        }
        Player.MoveSpeed = Player.NormalMoveSpeed;

        scoped = false;
        animator.SetBool("Scoped", false);
        Player.cam.GetComponent<Camera>().fieldOfView = previousFOV;
        Player.RotationSpeed = previousSense;
        gun.sprayModifier = previousSprayModifier;
        Player.weaponCamera.gameObject.SetActive(true);
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
        if (scopeOverlay != null)
        {
            scopeOverlay.SetActive(false);
        }
        gun.muzzleFlash.gameObject.SetActive(true);
    }

    private IEnumerator ScopeSniper()
    {
        crosshair.SetActive(false);

        yield return new WaitForSeconds(gun.scopeTime);

        scopeOverlay.SetActive(true);
        Player.weaponCamera.gameObject.SetActive(false);

        previousFOV = Player.cam.GetComponent<Camera>().fieldOfView;
        Player.cam.GetComponent<Camera>().fieldOfView = gun.scopeFOV;
        previousSense = Player.RotationSpeed;
        float ratio = Player.cam.GetComponent<Camera>().fieldOfView / previousFOV;
        Player.RotationSpeed *= Mathf.Pow((1.2f - 0.2f*ratio), 5) * ratio;
        gun.sprayModifier = gun.scopedSprayModifier;
    }

    private IEnumerator ScopeIronsight()
    {
        yield return new WaitForSeconds(gun.scopeTime);

        previousFOV = Player.cam.GetComponent<Camera>().fieldOfView;
        Player.cam.GetComponent<Camera>().fieldOfView = gun.scopeFOV;
        previousSense = Player.RotationSpeed;
        float ratio = Player.cam.GetComponent<Camera>().fieldOfView / previousFOV;
        Player.RotationSpeed *= Mathf.Pow((1.2f - 0.2f * ratio), 5) * ratio;
        gun.sprayModifier = gun.scopedSprayModifier;
    }

    private void OnDisable()
    {
        scoped = false;
        Player.cam.GetComponent<Camera>().fieldOfView = previousFOV;
        Player.RotationSpeed = previousSense;
        gun.sprayModifier = previousSprayModifier;
        Player.weaponCamera.gameObject.SetActive(true);
        animator.SetBool("Scoped", false);

        gun.muzzleFlash.gameObject.SetActive(true);

        if (scopeOverlay != null)
        {
            scopeOverlay.SetActive(false);
        }

        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }

}
public enum ScopeMode { Sniper, Ironsight, RedDot, None }