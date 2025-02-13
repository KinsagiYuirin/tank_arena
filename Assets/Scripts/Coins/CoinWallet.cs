using System;
using UnityEngine;
using Unity.Netcode;

public class CoinWallet : NetworkBehaviour
{

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<Coin>(out Coin coin))
        { return;}

        int coinValue = coin.Collect();
        
        if (!IsServer) {return;}

        TotalCoins.Value += coinValue;
    }
}
