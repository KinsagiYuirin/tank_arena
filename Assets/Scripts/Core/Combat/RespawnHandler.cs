using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }
    
    private void HandlePlayerSpawned(TankPlayer players)
    {
        players.Health.OnDie += (health) => HandlePlayerDie(player);
    }
    private void HandlePlayerDespawned(TankPlayer players)
    {
        
    }
    private void HandlePlayerDie(TankPlayer player)
    {
        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        throw new System.NotImplementedException();
    }
}
