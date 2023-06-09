using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPitComponent : MonoBehaviour
{
    [SerializeField] float respawnTimer = 2;
    [SerializeField] Transform respawnLocation;
    [SerializeField] float respawnIFramesTime = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            StartCoroutine(RespawnPlayer(collision.gameObject)); 
        }
        else
        {
            StartCoroutine(DisableCharacter(collision.gameObject));
        }
    }

    IEnumerator RespawnPlayer(GameObject player)
    {
        player.transform.Translate(0, 0, 0.5f);
        PlayerMoveComponent playerMove = player.GetComponent<PlayerMoveComponent>();
        playerMove.inDeathPit = true;
        yield return new WaitForSeconds(respawnTimer);
        playerMove.inDeathPit = false;
        player.transform.position = respawnLocation.position;
        player.GetComponent<PlayerDamageComponent>().TriggerInvincibility(respawnIFramesTime);
    }
    IEnumerator DisableCharacter(GameObject character)
    {
        yield return new WaitForSeconds(respawnTimer);
        character.transform.parent.gameObject.SetActive(false);
    }
}

