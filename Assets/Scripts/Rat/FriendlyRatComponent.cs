using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FriendlyRatComponent : MonoBehaviour
{
    GameObject player;
    PlayerTradeComponent trade;
    bool hasTraded = false;
    [SerializeField] GameObject tradeWindow;
    [SerializeField] float tradeWindowDisplayDistance = 10;
    public float SqrDistanceFromPlayer { get; private set; }
    private void Awake()
    {
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
        Debug.Log("Trading");
        Deselect();
        hasTraded = true;
    }
}
