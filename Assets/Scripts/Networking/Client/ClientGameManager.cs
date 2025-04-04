using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation allocation;
    private NetworkClient networkClient;
    private MatchplayMatchmaker matchmaker;
    private UserData userData;
    
    private const string MenuSceneName = "Menu";
    
    public void Disconnect()
    {
        networkClient?.Disconnect();
    }
    
    public void Dispose()
    {
        networkClient?.Dispose();
    }
    
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        
        networkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new MatchplayMatchmaker();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            userData = new UserData
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                userAuthId = AuthenticationService.Instance.PlayerId,
                userColorIndex = PlayerPrefs.GetInt(ColorSelector.PlayerColorKey,0)
            };
            return true;
        }
        
        return false;
    }

    public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakResponse)
    {
        if (matchmaker.IsMatchmaking)
        {
            return;
        }

        MatchmakerPollingResult matchResult = await GetMatchAsync();
        onMatchmakResponse?.Invoke(matchResult);
    }
    
    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchmaker.Matchmake(userData);

        if (matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            if (matchmakingResult.result == MatchmakerPollingResult.Success)
            {
                StartClient(matchmakingResult.ip, matchmakingResult.port);
            }
        }
        return matchmakingResult.result;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
    
    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }
        
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
        transport.SetRelayServerData(relayServerData);
        
        ConnectClient();
    }

    private void ConnectClient()
    {
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }

    public void StartClient(string ip, int port)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        ConnectClient();
    }

    public async Task CancelMatchmaking()
    {
        await matchmaker.CancelMatchmaking();
    }
}
