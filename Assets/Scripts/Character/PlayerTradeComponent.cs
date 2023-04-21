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

    private void Awake()
    {
        inputs = GetComponent<PlayerInputsComponent>();
    }

    // Called before all FriendlyRatComponent updates
    void Update()
    {
        previousTrader = potentialTrader;
        potentialTrader = null;
    }

    private void LateUpdate()
    {
        if (potentialTrader != null && canTrade)
        {
            if (inputs.InteractInput)
            {
                Trade();
            }
            else if (previousTrader != null && previousTrader != potentialTrader)
            {
                previousTrader.Deselect();
                potentialTrader.Select();
            }
            else
                potentialTrader.Select();
        }
        else if (previousTrader != null)
            previousTrader.Deselect();
    }
    public void ProposeTrade(FriendlyRatComponent friendlyRat)
    {
        if (potentialTrader == null || friendlyRat.SqrDistanceFromPlayer < potentialTrader.SqrDistanceFromPlayer)
            potentialTrader = friendlyRat;
    }
    private void Trade()
    {
        if (cheeseCount <= 0)
            return;
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cheese"))
        {
            cheeseCount++;
        }
    }
}
