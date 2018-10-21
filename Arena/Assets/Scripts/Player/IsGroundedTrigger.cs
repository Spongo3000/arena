using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGroundedTrigger : MonoBehaviour {

    private PlayerController player;


    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }
}
