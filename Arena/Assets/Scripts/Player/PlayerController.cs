using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerController : MonoBehaviour {

    public PhotonView PhotonView { get; private set; }
    private CharacterController cc;
    private TeamMember teamMember;
    public GunManager gunManager { get; private set; }
    public GameObject[] Meshes;
    public TextMesh NameplateTextMesh;
    public CrosshairHitmarker crosshairHitmarker { get; private set; }
    public Loadout loadout;
    public UnityEngine.UI.Dropdown LoadoutDropdown;
    public UnityEngine.UI.Slider RotationSpeedSlider;
    public GameObject LeaveGameButton;

    public GameObject cam;
    public GameObject DeathCamera;
    public Camera weaponCamera;

    public float Health = 100f;

    [HideInInspector] public float MoveSpeed;
    public float NormalMoveSpeed = 2f;
    public float SneakMultiplier = 0.45f;
    public float JumpForce = 3f;
    private float RemainingJumpForce = 1f;
    public float RotationSpeed = 1.5f;
    public Vector2 RotationRange = new Vector2(-90, 90);
    public bool Dead = false;

    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private Vector3 velocity = Vector3.zero;
    public GameObject Minimap;
    private Vector3 previousVelocity;
    [SerializeField] private AudioClip StepSound1;
    [SerializeField] private AudioClip StepSound2;
    private float nextStepSoundTime = 0f;
    private int nextStepSound = 1;
    private PlayerHUD HUD;

    private bool Sneaking { get { return MoveSpeed == NormalMoveSpeed * SneakMultiplier; } }


    private void Start()
    {
        MoveSpeed = NormalMoveSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PhotonView = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
        teamMember = GetComponent<TeamMember>();
        gunManager = GetComponentInChildren<GunManager>();
        DeathCamera = PlayerManager.Instance.DeathCamera;
        crosshairHitmarker = GetComponentInChildren<CrosshairHitmarker>();
        HUD = GetComponentInChildren<PlayerHUD>();

        NameplateTextMesh.text = PlayerNetwork.Instance.PlayerName;
        NameplateTextMesh.gameObject.SetActive(false);

        if (PhotonView.isMine)
        {
            cam.SetActive(true);
        }
        else
        {
            cam.GetComponent<Camera>().enabled = false;
            cam.GetComponent<FlareLayer>().enabled = false;
            cam.GetComponent<AudioListener>().enabled = false;
            weaponCamera.enabled = false;

            for (int i = 0; i < gunManager.transform.childCount; i++)
            {
                gunManager.transform.GetChild(i).gameObject.layer = 0;
                foreach (Transform t in gunManager.transform.GetChild(i))
                {
                    t.gameObject.layer = 0;
                }
            }
        }
        Loadout l = LoadoutManager.Instance.GetLoadout(1, gunManager.transform);
        loadout = l;
        LoadoutDropdown.gameObject.SetActive(false);

        RotationSpeed = PlayerPrefs.GetFloat("Sensitivity", 1f);
        RotationSpeedSlider.value = RotationSpeed;
        RotationSpeedSlider.value = RotationSpeed;
    }

    private void Update()
    {
        if (PhotonView.isMine)
        {
            if (RemainingJumpForce < 1f && cc.isGrounded)
            {
                RemainingJumpForce += Time.deltaTime;
                RemainingJumpForce = Mathf.Clamp(RemainingJumpForce, 0f, 1f);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                RotationSpeedSlider.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                LeaveGameButton.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                RotationSpeedSlider.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                LeaveGameButton.SetActive(false);
            }

            if (!Dead)
            {
                if (DeathCamera.activeSelf)
                {
                    DeathCamera.SetActive(false);
                }
                if (!cam.activeSelf)
                {
                    cam.SetActive(true);
                }
                if (LoadoutDropdown.gameObject.activeSelf)
                {
                    LoadoutDropdown.gameObject.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                if (!Minimap.activeSelf)
                {
                    Minimap.SetActive(true);
                }
                Move();
                if (!Input.GetKey(KeyCode.Escape))
                {
                    Rotate();
                }
                if (ScoreboardManager.Instance != null)
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        ScoreboardManager.Instance.ShowScoreboard();
                    }
                    if (Input.GetKeyUp(KeyCode.Tab))
                    {
                        ScoreboardManager.Instance.HideScoreboard();
                    }
                }
            }
            else
            {
                if (cam.activeSelf != false)
                {
                    cam.SetActive(false);
                }
                if (DeathCamera.activeSelf != true)
                {
                    DeathCamera.SetActive(true);
                }
                if (!LoadoutDropdown.gameObject.activeSelf)
                {
                    LoadoutDropdown.gameObject.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
                if (Minimap.activeSelf)
                {
                    Minimap.SetActive(false);
                }
                velocity = Vector3.zero;
                MoveSpeed = NormalMoveSpeed;
            }
        }
    }

    public void OnChange_GetLoadout()
    {
        loadout = LoadoutManager.Instance.LoadLoadout(LoadoutDropdown.value + 1);
        foreach (Transform gun in gunManager.transform)
        {
            if (loadout.PrimaryWeaponName == gun.name || loadout.SecondaryWeaponName == gun.name)
            {
                gun.GetComponent<Gun>().enabled = true;
                if (loadout.PrimaryWeaponName == gun.name)
                {
                    gun.gameObject.SetActive(true);
                }
            }
            else
            {
                gun.GetComponent<Gun>().enabled = false;
                gun.gameObject.SetActive(false);
            }
        }
        gunManager.SelectWeapon(loadout.PrimaryWeaponName);
    }

    public void OnChange_RotationSpeed()
    {
        RotationSpeed = RotationSpeedSlider.value;
        PlayerPrefs.SetFloat("Sensitivity", RotationSpeed);

        foreach (Scope scope in gunManager.transform.GetComponentsInChildren<Scope>())
        {
            scope.previousSense = RotationSpeed;
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(Health);
            stream.SendNext(GetComponent<TeamMember>().Team);
            stream.SendNext(Meshes[0].activeSelf);
            stream.SendNext(Meshes[1].activeSelf);
            stream.SendNext(NameplateTextMesh.text);
            if (gunManager != null)
            {
                foreach (Transform weapon in gunManager.transform)
                {
                    stream.SendNext(weapon.gameObject.activeSelf);
                }
            }
        }
        else
        {
            Health = (float)stream.ReceiveNext();
            GetComponent<TeamMember>().SetTeam((Team)stream.ReceiveNext());
            Meshes[0].SetActive((bool)stream.ReceiveNext());
            Meshes[1].SetActive((bool)stream.ReceiveNext());
            NameplateTextMesh.text = (string)stream.ReceiveNext();
            if (gunManager != null)
            {
                foreach (Transform weapon in gunManager.transform)
                {
                    weapon.gameObject.SetActive((bool)stream.ReceiveNext());
                }
            }
        }
    }

    private void Move()
    {
        float forwardVelocity = Input.GetAxisRaw("Vertical") * MoveSpeed;
        float sideVelocity = Input.GetAxisRaw("Horizontal") * MoveSpeed;

        if (Input.GetKeyDown(KeyCode.Space) && cc.isGrounded && RemainingJumpForce >= 0.25f)
        {
            verticalVelocity = JumpForce * RemainingJumpForce;
            RemainingJumpForce /= 2f;
        }
        else if (!cc.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        if (cc.isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                MoveSpeed = NormalMoveSpeed * SneakMultiplier;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) && !gunManager.CurrentGun.Scoped)
            {
                MoveSpeed = NormalMoveSpeed;
            }
            previousVelocity = Vector3.zero;
            velocity = new Vector3(sideVelocity, verticalVelocity, forwardVelocity);
            velocity = velocity.normalized * MoveSpeed;
            velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);
            velocity = transform.rotation * velocity;

            if (nextStepSoundTime <= 0f && !Sneaking)
            {
                if (forwardVelocity >= 0.2f || sideVelocity >= 0.2f)
                {
                    PlayStepSound();
                    nextStepSoundTime = 0.45f;
                }
            }
            else
            {
                nextStepSoundTime -= Time.deltaTime;
            }
        }
        else
        {
            if (previousVelocity == Vector3.zero)
            {
                previousVelocity = cc.velocity;
            }
            Vector3 airControlVector = new Vector3(Input.GetAxis("Horizontal") / 3, 0, Input.GetAxis("Vertical") / 3);
            airControlVector = transform.rotation * airControlVector;
            velocity = previousVelocity + airControlVector;
            velocity.y = verticalVelocity;
        }
    }

    private void FixedUpdate()
    {
        cc.Move(velocity * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        //Player's rotation
        float rotX = Input.GetAxis("Mouse X") * RotationSpeed;
        transform.Rotate(0, rotX, 0);

        //Camera rotation
        verticalRotation -= Input.GetAxis("Mouse Y") * RotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, RotationRange.x, RotationRange.y);
        cam.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void Die(string damageSender)
    {
        if (!PhotonView.isMine)
        {
            return;
        }
        KillfeedManager.Instance.AddNewElement(damageSender, NameplateTextMesh.text);
        if (ScoreboardManager.Instance != null)
        {
            ScoreboardManager.Instance.IncreaseKills(damageSender);
            ScoreboardManager.Instance.IncreaseDeaths(NameplateTextMesh.text);
        }
        PlayerManager.Instance.Respawn(this);
    }

    [PunRPC]
    private void RPC_TakeDamage(float value, string sender)
    {
        if (PhotonView.isMine)
        {
            HUD.PlayVignette();
        }
        Health -= value;
        if (Health <= 0f && !Dead)
        {
            Dead = true;
            Die(sender);
        }
    }

    public void TakeDamage(float value, string damageSender)
    {
        PhotonView.RPC("RPC_TakeDamage", PhotonTargets.All, value, damageSender);
    }

    [PunRPC]
    private void RPC_PlaySound(int index)
    {
        GetComponent<AudioSource>().PlayOneShot(gunManager.transform.GetChild(index).GetComponent<Gun>().sound);
    }

    [PunRPC]
    private void RPC_PlayStepSound()
    {
        AudioClip toBePlayed;
        if (nextStepSound == 1)
        {
            toBePlayed = StepSound1;
            nextStepSound++;
        }
        else
        {
            toBePlayed = StepSound2;
            nextStepSound--;
        }

        GetComponent<AudioSource>().PlayOneShot(toBePlayed);
    }

    private void PlayStepSound()
    {
        PhotonView.RPC("RPC_PlayStepSound", PhotonTargets.All, null);
    }

    public void PlaySound(int index)
    {
        PhotonView.RPC("RPC_PlaySound", PhotonTargets.All, index);
    }

    public void IncreaseVerticalRotation(float value)
    {
        verticalRotation -= value;
        verticalRotation = Mathf.Clamp(verticalRotation, RotationRange.x, RotationRange.y);
    }

    public void OnClick_LeaveMatch()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(GameSettings.MainMenuScene);
    }
}
