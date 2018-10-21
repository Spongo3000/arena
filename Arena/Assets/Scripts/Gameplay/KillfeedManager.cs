using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KillfeedManager : MonoBehaviour {

    public static KillfeedManager Instance;

    private PhotonView PhotonView;

    public float deleteTime = 5f;
    public GameObject KillfeedLayoutGroup;
    public GameObject KillfeedLayoutElementPrefab;


    private void Awake()
    {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    private void RPC_AddNewElement(string killer, string killed)
    {
        GameObject element = Instantiate(KillfeedLayoutElementPrefab, KillfeedLayoutGroup.transform);
        element.GetComponent<Text>().text = "<color=#ff0000>" + killer + "</color>" + " killed <color=#0000ff>" + killed + "</color>";
        Destroy(element, deleteTime);
    }

    public void AddNewElement(string killer, string killed)
    {
        PhotonView.RPC("RPC_AddNewElement", PhotonTargets.All, killer, killed);
    }
}
