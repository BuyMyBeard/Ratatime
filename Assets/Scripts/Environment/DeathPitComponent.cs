using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPitComponent : MonoBehaviour
{
    [SerializeField] float respawnTimer = 2;
    [SerializeField] Transform respawnLocation;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            StartCoroutine(RespawnPlayer(collision.gameObject));
        }
        else
        {
            collision.gameObject.SetActive(false);
        }
    }

    IEnumerator RespawnPlayer(GameObject player)
    {
        PlayerMoveComponent playerMove = player.GetComponent<PlayerMoveComponent>();
        playerMove.inDeathPit = true;
        yield return new WaitForSeconds(respawnTimer);
        playerMove.inDeathPit = false;
        player.transform.position = respawnLocation.position;
    }
}

