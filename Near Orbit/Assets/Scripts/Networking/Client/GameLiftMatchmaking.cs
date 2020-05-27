using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.GameLift;
using Amazon.GameLift.Model;

public class GameLiftMatchmaking
{

    private AmazonGameLiftClient ClientInstance;
    private string CurrentTicketId;

    public GameLiftMatchmaking(AmazonGameLiftClient client)
    {
        ClientInstance = client;
    }

    public void RequestMatch(string mmConfig, List<Player> players)
    {
        CurrentTicketId = GenerateTicketID();

        var startMatchmakingRequest = new StartMatchmakingRequest()
        {
            ConfigurationName = mmConfig,
            TicketId = CurrentTicketId,
            Players = players
        };

        StartMatchmakingResponse startMatchmakingResponse;
        try
        {
            startMatchmakingResponse = ClientInstance.StartMatchmaking(startMatchmakingRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return;
        }

        Debug.LogFormat("Successfully created matchmaking request with ticket {0}", CurrentTicketId);
    }

    public void CancelMatchRequest()
    {
        var stopMatchmakingRequest = new StopMatchmakingRequest()
        {
            TicketId = CurrentTicketId
        };

        StopMatchmakingResponse stopMatchmakingResponse;
        try
        {
            stopMatchmakingResponse = ClientInstance.StopMatchmaking(stopMatchmakingRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return;
        }

        Debug.LogFormat("Successfully canceled matchmaking request with ticket {0}", CurrentTicketId);
    }

    public MatchmakingTicket CheckMatchRequest()
    {
        var describeMatchmakingRequest = new DescribeMatchmakingRequest()
        {
            TicketIds = new List<string>()
            {
                CurrentTicketId
            }
        };

        DescribeMatchmakingResponse describeMatchmakingResponse;
        try
        {
            describeMatchmakingResponse = ClientInstance.DescribeMatchmaking(describeMatchmakingRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return null;
        }

        if (describeMatchmakingResponse.TicketList.Count > 0)
        {
            return describeMatchmakingResponse.TicketList[0];
        }
        else
        {
            Debug.LogError("Describe matchmaking response ticket list empty!");
            return null;
        }
    }

    public string GenerateTicketID()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz012345678901234567890123456789";
        char[] stringChars = new char[64];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }

}
