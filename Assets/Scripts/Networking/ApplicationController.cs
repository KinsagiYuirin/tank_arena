using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    
    private ApplicationData appData;
    private const string GameSceneName = "Game";
    
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await LaunchInMode(SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null);
    }
    
    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            Application.targetFrameRate = 60;
            appData = new ApplicationData();
            ServerSingleton serverSingleton = Instantiate(serverPrefab);
            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(playerPrefab);
            
            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }
    
    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameSceneName);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        #if UNITY_SERVER
        Task createServerTask = serverSingleton.CreateServer(playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        Task startServerTask = serverSingleton.GameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
        #endif
    }

}
