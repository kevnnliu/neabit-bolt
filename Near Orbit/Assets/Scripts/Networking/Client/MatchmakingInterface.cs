using Amazon.GameLift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                var RequestTicket = Client.CheckMatchRequest();

                if (RequestTicket != null)
                {
                    IsSearching = false;
                    
                    if (RequestTicket.Status == MatchmakingConfigurationStatus.COMPLETED)
                    {
                        var gameSessionInfo = RequestTicket.GameSessionConnectionInfo;
                        string gameSessionARN = gameSessionInfo.GameSessionArn;
                        string prefix = string.Format("arn:aws:gamelift:us-west-2::gamesession/{0}/", GameLiftClient.FLEET_ID);
                        string gameSessionId = gameSessionARN.Substring(prefix.Length);

                        Debug.LogFormat("Game session ARN: {0}", gameSessionARN);
                        Debug.LogFormat("Game session ID: {0}", gameSessionId);

                        Client.CreatePlayerSession(Client.GetGameSession(gameSessionId), joinImmediately: true);
                    }
                    else if (RequestTicket.Status == MatchmakingConfigurationStatus.FAILED)
                    {
                        CancelMatchRequest();
                        Debug.LogWarningFormat("Matchmaking request failed: {0}", RequestTicket.StatusMessage);
                    }
                    else
                    {
                        IsSearching = true;
                        Debug.LogFormat("Match request status: {0}", RequestTicket.Status);
                    }
                }
                
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

}
