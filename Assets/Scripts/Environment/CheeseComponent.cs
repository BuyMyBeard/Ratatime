using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseComponent : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float frequency;
    float elapsedTime = 0;
    float previousDisplacement = 0;
    AudioManagerComponent audioManager;
    public bool IsCollected { get; private set; } = false;

    private void Awake()
    {
        audioManager = GetComponent<AudioManagerComponent>();
        StartCoroutine(MoveOverTime());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IsCollected = true;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        collision.GetComponent<PlayerTradeComponent>().CollectCheese();
        StopAllCoroutines();
        audioManager.PlaySFX(0);
    }
    IEnumerator MoveOverTime()
    {
        while (true)
        {
            float displacement = amplitude * Mathf.Sin(frequency * 2 * Mathf.PI * (elapsedTime += Time.deltaTime));
            transform.Translate((displacement - previousDisplacement) * Vector2.up);
            previousDisplacement = displacement;
            yield return null;
        }
    }
}
