using System;
using UnityEngine;
using Unity.Netcode;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    
    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;
    
    private bool shouldFire;
    //private float previousFireTime;
    private float muzzleFlashTimer;

    private float timer;
    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }
    
    private void Update()
    {
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }
        if (!IsOwner) { return; }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (!shouldFire) { return; }
        if (timer > 0) { return; }

        if (wallet.TotalCoins.Value < costToFire)
        { return; }
        
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1 / fireRate;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }
    
    [Rpc(SendTo.Server)]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (wallet.TotalCoins.Value < costToFire) {return;}
        wallet.SpendCoins(costToFire);
        
        GameObject projectileInstance = Instantiate(
            serverProjectilePrefab, 
            spawnPos, 
            Quaternion.identity);
        
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }
        
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
        
        SpawnDummyProjectileRpc(spawnPos, direction);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnDummyProjectileRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner)
        { return; }
        
        SpawnDummyProjectile(spawnPos, direction);
    }
    
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;
            
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
        
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
    }
}
