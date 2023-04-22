using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatHitboxComponent : MonoBehaviour
{
    [SerializeField] CapsuleCollider2D uprightHitbox;
    [SerializeField] CapsuleCollider2D crawlingHitbox;

    bool isStanding = true;

    //you can use either method you prefer
    public void Toggle()
    {
        if (isStanding)
            Crawl();
        else
            Stand();
    }
    public void Crawl()
    {
        uprightHitbox.enabled = false;
        crawlingHitbox.enabled = true;
        isStanding = false;
    }
    public void Stand()
    {
        uprightHitbox.enabled = true;
        crawlingHitbox.enabled = false;
        isStanding = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //do attack animation or something
    }

}
