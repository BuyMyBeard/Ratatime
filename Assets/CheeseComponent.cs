using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseComponent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;     
    }

}
