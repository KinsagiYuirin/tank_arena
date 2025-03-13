using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using Unity.Collections;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private CinemachineCamera virtualCamera;

    [Header("Setting")] 
    [SerializeField] private int ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            PlayerName.Value = userData.userName;
        }
        
        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
}
