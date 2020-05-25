using System;
using System.Collections.Generic;
using UnityEngine;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using Bolt;
using UdpKit;
using Bolt.Matchmaking;
using Bolt.Photon;

public class GameLiftClient : GlobalEventListener
{

    private const string GameLiftFleetId = "testfleetid";
    private const string AccessKey = "AKIAQMSDIM7CTX4W3UWH";
    private const string SecretKey = "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";

    private AmazonGameLiftConfig ClientConfig;
    private Credentials ClientCredentials;
    private AmazonGameLiftClient ClientInstance;

    private GameSession CurrentGameSession;
    private PlayerSession CurrentPlayerSession;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        //StartGameLiftClient();

        //List<GameProperty> gameProperties = new List<GameProperty>
        //    {
        //        new GameProperty
        //        {
        //            Key = "m",
        //            Value = "NetworkTest"
        //        }
        //    };
        //CreateGameSession(4, "asdfasdfasdfasdf", gameProperties, true);
        BoltLauncher.StartClient();
        Debug.Log("Started Bolt client!");
    }

    public void StartGameLiftClient()
    {
        ClientConfig = new AmazonGameLiftConfig
        {
            ServiceURL = "http://localhost:9080" //Use for local testing, comment out for live build
            //RegionEndpoint = RegionEndpoint.USWest2 //Use for live build, comment out for local testing
        };

        ClientCredentials = new Credentials
        {
            AccessKeyId = AccessKey,
            SecretAccessKey = SecretKey
        };

        ClientInstance = new AmazonGameLiftClient(ClientCredentials, ClientConfig);
        Debug.Log("Started GameLift client!");
    }

    public override void BoltStartBegin()
    {
        // Register any IProtocolToken that you are using
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        BoltNetwork.RegisterTokenClass<ClientToken>();
        BoltNetwork.RegisterTokenClass<ProjectileToken>();

        Debug.Log("Finished registering custom token classes");
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsClient)
        {
            //ClientToken token = new ClientToken
            //{
            //    UserId = Launcher.UserID,
            //    Username = Launcher.Username,
            //    PlayerSessionId = CurrentPlayerSession.PlayerSessionId
            //};
            //Debug.LogFormat("Created client token with player session {0}", token.PlayerSessionId);

            ////UdpEndPoint endPoint = new UdpEndPoint(UdpIPv4Address.Parse(CurrentGameSession.IpAddress), (ushort)CurrentGameSession.Port);
            //BoltMatchmaking.JoinSession(CurrentGameSession.GameSessionId, token);
            //Debug.LogFormat("Joined Bolt session {0}", BoltMatchmaking.CurrentSession.Id);
            ClientToken token = new ClientToken
            {
                UserId = Launcher.UserID,
                Username = Launcher.Username,
                PlayerSessionId = "psess-12345"
            };
            Debug.LogFormat("Created client token with player session {0}", token.PlayerSessionId);
            BoltMatchmaking.JoinSession("asdfasdfasdf", token);
            Debug.LogFormat("Joined Bolt session {0}", BoltMatchmaking.CurrentSession.Id);
        }
        else
        {
            Debug.LogError("Attempted to join Bolt session not as client!");
        }
    }

    public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
    {
        Debug.LogErrorFormat("BoltStartFailed. Reason: {0}", disconnectReason);
    }

    #region GameLift Game Sessions

    public void CreateGameSession(int maxPlayers, string sessionToken, List<GameProperty> gameProperties, bool joinImmediately)
    {
        var gameSessionRequest = new CreateGameSessionRequest
        {
            IdempotencyToken = sessionToken,
            MaximumPlayerSessionCount = maxPlayers,
            GameProperties = gameProperties,
            FleetId = GameLiftFleetId
        };

        CreateGameSessionResponse gameSessionResponse;
        try
        {
            gameSessionResponse = ClientInstance.CreateGameSession(gameSessionRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return;
        }

        if (gameSessionResponse == null)
        {
            Debug.LogError("Could not create game session!");
            Debug.LogError(gameSessionResponse.ResponseMetadata.ToString());
        }
        else
        {
            Debug.LogFormat("Successfully created game session {0}", gameSessionResponse.GameSession.GameSessionId);
            if (joinImmediately)
            {
                CurrentGameSession = gameSessionResponse.GameSession;
                CreatePlayerSession(CurrentGameSession, true);
            }
        }
    }

    /// <summary>
    /// Finds and joins the first available game session based on filterQuery and sortQuery.
    /// </summary>
    /// <param name="filterQuery">String containing the search criteria for the session search. 
    /// If no filter expression is included, the request returns results for all game sessions in the fleet that are in ACTIVE status.</param>
    /// <param name="sortQuery">Instructions on how to sort the search results. 
    /// If no sort expression is included, the request returns results in random order.</param>
    public void FindGameSession(string filterQuery, string sortQuery, bool joinImmediately)
    {
        var gameSessionsRequest = new SearchGameSessionsRequest
        {
            FleetId = GameLiftFleetId,
            FilterExpression = filterQuery,
            SortExpression = sortQuery,
            Limit = 1
        };

        SearchGameSessionsResponse gameSessionsResponse;
        try
        {
            gameSessionsResponse = ClientInstance.SearchGameSessions(gameSessionsRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return;
        }

        if (gameSessionsResponse == null)
        {
            Debug.LogErrorFormat("Could not find any game session with the following parameters: {0}", filterQuery);
        }
        else
        {
            Debug.LogFormat("Successfully found game session with the following parameters: {0}", filterQuery);
            if (joinImmediately)
            {
                CreatePlayerSession(gameSessionsResponse.GameSessions[0], true);
            }
        }
    }

    public void CreatePlayerSession(GameSession selectedGameSession, bool joinImmediately)
    {
        Debug.LogFormat("{0}/{1} players in selected game session.", selectedGameSession.CurrentPlayerSessionCount, selectedGameSession.MaximumPlayerSessionCount);
        CurrentGameSession = selectedGameSession;

        var playerSessionRequest = new CreatePlayerSessionRequest
        {
            GameSessionId = selectedGameSession.GameSessionId,
            PlayerData = Launcher.Username,
            PlayerId = Launcher.UserID
        };

        CreatePlayerSessionResponse playerSessionResponse;
        try
        {
            playerSessionResponse = ClientInstance.CreatePlayerSession(playerSessionRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return;
        }

        if (playerSessionResponse == null)
        {
            Debug.LogErrorFormat("Unable to create session for player {0} with game session {1}", Launcher.UserID, selectedGameSession.GameSessionId);
        }
        else
        {
            CurrentPlayerSession = playerSessionResponse.PlayerSession;
            Debug.LogFormat("Successfully created player session {0} with game session {1}", CurrentPlayerSession.PlayerSessionId, selectedGameSession.GameSessionId);
            if (joinImmediately)
            {
                BoltLauncher.StartClient();
                Debug.Log("Started Bolt client!");
            }
        }
    }

    #endregion

}
