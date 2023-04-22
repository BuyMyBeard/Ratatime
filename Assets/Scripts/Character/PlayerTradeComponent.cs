using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTradeComponent : MonoBehaviour
{
    [SerializeField] int cheeseCount = 1;
    [SerializeField] float tradeCooldown = 1;
    FriendlyRatComponent potentialTrader = null, previousTrader;
    PlayerInputsComponent inputs;
    bool canTrade = true;
    AudioManagerComponent audioManager;
    private void Awake()
    {
        inputs = GetComponent<PlayerInputsComponent>();
        audioManager = GetComponent<AudioManagerComponent>();
    }

    // Called before all FriendlyRatComponent updates
    void Update()
    {
        previousTrader = potentialTrader;
        potentialTrader = null;
    }

    private void LateUpdate()
    {
        if (potentialTrader == null || !canTrade)
        {
            if (previousTrader != null)
                previousTrader.Deselect();
            return;
        }

        if (inputs.InteractInput)
        {
            Trade();
        }
        else if (previousTrader != null && previousTrader != potentialTrader)
        {
            previousTrader.Deselect();
            potentialTrader.Select();
            audioManager.PlaySFX(1);
        }
        else
        {
            potentialTrader.Select();
        }

    }
    public void ProposeTrade(FriendlyRatComponent friendlyRat)
    {
        if (potentialTrader == null || friendlyRat.SqrDistanceFromPlayer < potentialTrader.SqrDistanceFromPlayer)
            potentialTrader = friendlyRat;
    }
    private void Trade()
    {
        if (cheeseCount <= 0)
        {
            audioManager.PlaySFX(2);
            StartCoroutine(TradeCooldown());
            return;
        }
        audioManager.PlaySFX(0);
        potentialTrader.Trade();
        potentialTrader = null;
        previousTrader = null;
        cheeseCount--;
        StartCoroutine(TradeCooldown());
    }

    IEnumerator TradeCooldown()
    {
        canTrade = false;
        yield return new WaitForSeconds(tradeCooldown);
        canTrade = true;
    }
    public void CollectCheese()
    {
        cheeseCount++;
    }
}
