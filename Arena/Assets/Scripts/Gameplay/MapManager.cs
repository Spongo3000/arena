using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public static MapManager Instance;

    public Transform SpawnpointsParent;


    private void Awake()
    {
        Instance = this;
    }

    public Transform GetRandomSpawnpoint()
    {
        int index = Random.Range(0, SpawnpointsParent.childCount);

        return SpawnpointsParent.GetChild(index);
    }
}
