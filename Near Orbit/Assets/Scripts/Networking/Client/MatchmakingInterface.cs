using Amazon.GameLift;
using Amazon.GameLift.Model;
using System.Collections;
using UnityEngine;

public class MatchmakingInterface : MonoBehaviour
{

    private const string MMCONFIG = "OregonMMConfig1";
    private const float CHECK_FREQ = 10f;

    [SerializeField]
    private GameObject SearchButton;

    [SerializeField]
    private GameObject CancelButton;

    [SerializeField]
    private GameObject SearchingText;

    private GameLiftClient Client;
    private bool IsSearching = false;
    private float CheckRequestTimer = CHECK_FREQ;

    private void Update()
    {
        if (IsSearching)
        {
            CheckRequestTimer -= Time.deltaTime;
            if (CheckRequestTimer <= 0)
            {
                CheckMatchRequest();
                
                CheckRequestTimer = CHECK_FREQ;
            }
        }
    }

    public void SetClient(GameLiftClient gameLiftClient)
    {
        Client = gameLiftClient;
    }

    public void RequestMatch()
    {
        Client.RequestMatch(MMCONFIG);
        IsSearching = true;
        UpdateInterface();
    }

    public void CancelMatchRequest()
    {
        Client.CancelMatchRequest();
        IsSearching = false;
        UpdateInterface();
    }

    private void UpdateInterface()
    {
        SearchButton.SetActive(!IsSearching);
        SearchingText.SetActive(IsSearching);
        CancelButton.SetActive(IsSearching);
    }

    private void CheckMatchRequest()
    {
        var requestTicket = Client.CheckMatchRequest();

        if (requestTicket != null)
        {
            IsSearching = false;

            if (requestTicket.Status == MatchmakingConfigurationStatus.COMPLETED)
            {
                ConnectToGameSession(requestTicket);
            }
            else if (requestTicket.Status == MatchmakingConfigurationStatus.FAILED)
            {
                CancelMatchRequest();
                Debug.LogWarningFormat("Matchmaking request failed: {0}", requestTicket.StatusMessage);
            }
            else
            {
                IsSearching = true;
                Debug.LogFormat("Match request status: {0}", requestTicket.Status);
            }
        }
    }

    private void ConnectToGameSession(MatchmakingTicket requestTicket)
    {
        var gameSessionInfo = requestTicket.GameSessionConnectionInfo;

        //We use the ARN because matchmaking creates game sessions with the ARN as the ID
        string gameSessionARN = gameSessionInfo.GameSessionArn;
        Debug.LogFormat("Game session ARN: {0}", gameSessionARN);

        string playerSessionId = requestTicket.GameSessionConnectionInfo.MatchedPlayerSessions[0].PlayerSessionId;
        Client.SetSessions(gameSessionARN, playerSessionId, startBoltClient: true);
    }
}
