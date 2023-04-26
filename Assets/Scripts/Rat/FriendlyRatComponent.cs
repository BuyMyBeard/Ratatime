using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FriendlyRatComponent : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController femaleAnimator;
    [SerializeField] TextMeshProUGUI cheeseCount, timeCount;
    [SerializeField] int minPrice, maxPrice, minCount, maxCount;
    AudioManagerComponent sfx;
    enum Animations { Idle, Looking, Blinking, Happy };
    readonly Dictionary<Animations, string> dictAnimations = new Dictionary<Animations, string>()
    {
        { Animations.Idle, "idle" },
        { Animations.Looking, "looking" },
        { Animations.Blinking, "blinking" },
        { Animations.Happy, "happy" },
    };

    public int Price { get; private set; }
    public int SellCount { get; private set; }

    GameObject player;
    PlayerTradeComponent trade;
    bool hasTraded = false;
    [SerializeField] GameObject tradeWindow;
    [SerializeField] float tradeWindowDisplayDistance = 10;
    Animator animator;
    SpriteRenderer sprite;
    int gender;
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
        sfx = GetComponent<AudioManagerComponent>();
        animator = GetComponent<Animator>();
        gender = Random.Range(0, 2);
        if (gender == 0)
            animator.runtimeAnimatorController = femaleAnimator;
        Price = Random.Range(minPrice, maxPrice + 1);
        cheeseCount.SetText(Price.ToString());
        SellCount = Random.Range(minCount, maxCount + 1);
        timeCount.SetText(SellCount.ToString());
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
    public void RefuseTrade()
    {
        if (gender == 0)
            sfx.PlaySFX(3);
        else
            sfx.PlaySFX(4);
    }

    IEnumerator TradeAnimation()
    {
        if (gender == 0)
        {
            if (Random.Range(0, 2) == 0)
                sfx.PlaySFX(0);
            else
                sfx.PlaySFX(1);
        }
        else
            sfx.PlaySFX(2);
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
