using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FriendlyRatComponent : MonoBehaviour
{
    [SerializeField] Animator femaleAnimator;

    enum Animations { Idle, Looking, Blinking, Happy };
    readonly Dictionary<Animations, string> dictAnimations = new Dictionary<Animations, string>()
    {
        { Animations.Idle, "idle" },
        { Animations.Looking, "looking" },
        { Animations.Blinking, "blinking" },
        { Animations.Happy, "happy" },
    };

    GameObject player;
    PlayerTradeComponent trade;
    bool hasTraded = false;
    [SerializeField] GameObject tradeWindow;
    [SerializeField] float tradeWindowDisplayDistance = 10;
    Animator animator;
    SpriteRenderer sprite;
    Animations currentAnimation = Animations.Idle;
    public float SqrDistanceFromPlayer { get; private set; }

    private void OnEnable()
    {
        StartCoroutine(Animate());
        int direction = Random.Range(0, 2);
        sprite.flipX = direction == 0;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        int gender = Random.Range(0, 2);
        sprite = GetComponent<SpriteRenderer>();
        tradeWindow.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        trade = player.GetComponent<PlayerTradeComponent>();
    }
    //can be optimized with coroutines if needed
    void Update()
    {
        if (hasTraded)
            return;
        SqrDistanceFromPlayer = (player.transform.position - transform.position).sqrMagnitude;
        bool playerInRange = SqrDistanceFromPlayer < tradeWindowDisplayDistance * tradeWindowDisplayDistance;
        if (playerInRange)
            trade.ProposeTrade(this);
    }

    public void Select()
    {
        tradeWindow.SetActive(true);
    }
    public void Deselect()
    {
        tradeWindow.SetActive(false);
    }
    public void Trade()
    {
        StartCoroutine(TradeAnimation());
        Deselect();
        hasTraded = true;
    }
    private void SetAnimation(Animations animation)
    {
        if (currentAnimation != animation)
        {
            animator.Play(dictAnimations[animation]);
            currentAnimation = animation;
        }
    }

    IEnumerator Animate()
    {
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (currentAnimation == Animations.Idle)
            {
                int choice = Random.Range(1, 11);
                if (choice < 2)
                    StartCoroutine(LookAnimation());
                else if (i++ % 5 == 3)
                    StartCoroutine(BlinkAnimation());
            }

        }
    }

    IEnumerator TradeAnimation()
    {
        SetAnimation(Animations.Happy);
        yield return new WaitForSeconds(2);
        SetAnimation(Animations.Idle);
    }
    IEnumerator BlinkAnimation()
    {
        SetAnimation(Animations.Blinking);
        yield return new WaitForSeconds(0.2f);
        SetAnimation(Animations.Idle);
    }
    IEnumerator LookAnimation()
    {
        SetAnimation(Animations.Looking);
        yield return new WaitForSeconds(2);
        SetAnimation(Animations.Idle);
    }
}
