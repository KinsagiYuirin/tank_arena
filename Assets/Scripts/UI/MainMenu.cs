using System;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text joinTimerText;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_InputField joinCodeField;

    private bool isMatchmaking;
    private bool isCancelling;
    
    private void Start()
    {
        if (ClientSingleton.Instance == null) {return;}
        
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        queueStatusText.text = string.Empty;
        joinTimerText.text = string.Empty;
    }
    
    public async void FindMatchPressed()
    {
        if (isCancelling) {return;}

        if (isMatchmaking)
        {
            queueStatusText.text = "Cancelling...";
            isCancelling = true;
            await ClientSingleton.Instance.GameManager.CancelMatchmaking();
            isCancelling = false;
            isMatchmaking = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            return;
        }
        ClientSingleton.Instance.GameManager.MatchmakeAsync(OnMatchMode);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching...";
        isMatchmaking = true;
    }

    private void OnMatchMode(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusText.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusText.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatusText.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusText.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusText.text = "MatchAssignmentError";
                break;
        }
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
        Debug.Log("Host started");
    }
    
    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}
