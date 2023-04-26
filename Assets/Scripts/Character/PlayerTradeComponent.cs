using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTradeComponent : MonoBehaviour
{
    [SerializeField] int cheeseCount = 1;
    [SerializeField] float tradeCooldown = 1;
    [SerializeField] float tradeRange = 0.5f;
    FriendlyRatComponent potentialTrader = null, previousTrader;

    PlayerInputsComponent inputs;
    bool canTrade = true;
    Game_Manager gameManager;

    public event EventHandler CheeseCountChanged;
    private void Awake()
    {
        inputs = GetComponent<PlayerInputsComponent>();
        gameManager = FindAnyObjectByType<Game_Manager>();
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

        if (inputs.InteractInput && potentialTrader.SqrDistanceFromPlayer < tradeRange * tradeRange)
        {
            Trade();
        }
        else if (previousTrader != null && previousTrader != potentialTrader)
        {
            previousTrader.Deselect();
            potentialTrader.Select();
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
        if (cheeseCount < potentialTrader.Price)
        {
            potentialTrader.RefuseTrade();
            StartCoroutine(TradeCooldown());
            return;
        }
        potentialTrader.Trade();
        cheeseCount -= potentialTrader.Price;
        gameManager.AddTime(potentialTrader.SellCount);
        potentialTrader = null;
        previousTrader = null;
        InvokeCheeseCountChanged();
        StartCoroutine(TradeCooldown());
    }

    void InvokeCheeseCountChanged ()
    {
        if (CheeseCountChanged != null)
        {
            CheeseCountChanged.Invoke(cheeseCount, null);
        }
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
        InvokeCheeseCountChanged();
    }
}
