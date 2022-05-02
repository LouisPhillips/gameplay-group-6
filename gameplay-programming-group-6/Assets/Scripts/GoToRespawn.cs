using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToRespawn : MonoBehaviour
{
    public PlayerMovement player;

    private void OnTriggerEnter(Collider other)
    {
        player.transform.position = player.respawnPoint;
    }
}
