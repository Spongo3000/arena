using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    private PlayerController Player;
    private GunManager gunManager;
    [SerializeField] private Animator vignetteAnimator;

    [Header("Health Bar")]
    public Image HealthbarImage;
    public Text HealthbarText;

    [Header("Ammo Bar")]
    public Image AmmobarImage;
    public Text AmmobarText;

    [Space]

    [Tooltip("This image lights up when you get hit.")]
    [SerializeField] private Image Vignette;


    private void Start()
    {
        Player = GetComponentInParent<PlayerController>();
        gunManager = Player.gunManager;
        if (!Player.PhotonView.isMine)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Gun selectedGun = gunManager.transform.GetChild(gunManager.selectedWeapon).GetComponent<Gun>();

        HealthbarImage.fillAmount = Player.Health / 100f;
        HealthbarText.text = Player.Health.ToString();

        AmmobarImage.fillAmount = selectedGun.ammo / selectedGun.maxAmmo;
        AmmobarText.text = selectedGun.ammo.ToString();
    }

    private IEnumerator IE_PlayVignette()
    {
        if (vignetteAnimator.GetBool("active"))
        {
            yield return null;
        }
        vignetteAnimator.SetBool("active", true);
        yield return new WaitForSeconds(0.15f);
        vignetteAnimator.SetBool("active", false);
    }

    public void PlayVignette()
    {
        StartCoroutine("IE_PlayVignette");
    }
}
