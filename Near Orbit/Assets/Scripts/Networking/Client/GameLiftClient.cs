using System;
using System.Collections.Generic;
using UnityEngine;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using Bolt;
using UdpKit;
using Bolt.Matchmaking;
using Bolt.Photon;
using Amazon;

public class GameLiftClient : GlobalEventListener
{

    public const string FLEET_ID = "fleet-4e479896-0e99-4324-a7d0-3c65905b1769";
    private const string ACCESS_KEY = "AKIAQMSDIM7CTX4W3UWH";
    private const string SECRET_KEY = "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";

    private AmazonGameLiftConfig ClientConfig;
    private Credentials ClientCredentials;
    private AmazonGameLiftClient ClientInstance;

    private GameSession CurrentGameSession;
    private PlayerSession CurrentPlayerSession;

    private GameLiftMatchmaking MatchmakingHandler;

    [SerializeField]
    private GameObject MatchmakingInterfacePrefab;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        StartGameLiftClient();
    }

    /// <summary>
    /// This sets the current game session and player session. This is to be used in conjunction with matchmaking.
    /// </summary>
    /// <param name="gameSessionId"></param>
    /// <param name="playerSession"></param>
    /// <param name="startBoltClient"></param>
    public void SetSessions(string gameSessionId, string playerSessionId, bool startBoltClient)
    {
        CurrentGameSession = new GameSession()
        {
            GameSessionId = gameSessionId
        };
        CurrentPlayerSession = new PlayerSession()
        {
            PlayerSessionId = playerSessionId,
            GameSessionId = gameSessionId,
            PlayerId = Launcher.UserID,
            PlayerData = Launcher.Username
        };

        if (startBoltClient)
        {
            BoltLauncher.StartClient();
        }
    }

    public void RequestMatch(string mmConfig)
    {
        //Hard-coded for testing purposes
        var player = new Player()
        {
            PlayerId = Launcher.UserID,
            Team = "duelist", 
            PlayerAttributes = new Dictionary<string, AttributeValue>()
            {
                {"skill", new AttributeValue() { N = 10 }}
            }
        };

        var playerList = new List<Player>()
        {
            player
        };

        MatchmakingHandler.RequestMatch(mmConfig, playerList);
    }

    public void CancelMatchRequest()
    {
        MatchmakingHandler.CancelMatchRequest();
    }

    public MatchmakingTicket CheckMatchRequest()
    {
        return MatchmakingHandler.CheckMatchRequest();
    }

    public void StartGameLiftClient()
    {
        ClientConfig = new AmazonGameLiftConfig
        {
            //ServiceURL = "http://localhost:9080" //Uncomment for local testing, comment out for live build
            RegionEndpoint = RegionEndpoint.USWest2 //Uncomment for live build, comment out for local testing
        };

        ClientCredentials = new Credentials
        {
            AccessKeyId = ACCESS_KEY,
            SecretAccessKey = SECRET_KEY
        };

        ClientInstance = new AmazonGameLiftClient(ClientCredentials, ClientConfig);
        Debug.Log("Started GameLift client!");

        MatchmakingHandler = new GameLiftMatchmaking(ClientInstance);
        Debug.Log("Started matchmaking handler!");

        GameObject matchmakingInterfaceObject = Instantiate(MatchmakingInterfacePrefab);
        matchmakingInterfaceObject.GetComponent<MatchmakingInterface>().SetClient(this);
    }

    #region Bolt Callbacks

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
            ClientToken token = new ClientToken
            {
                UserId = Launcher.UserID,
                Username = Launcher.Username,
                PlayerSessionId = CurrentPlayerSession.PlayerSessionId
            };
            Debug.LogFormat("Created client token with player session {0}", token.PlayerSessionId);

            //UdpEndPoint endPoint = new UdpEndPoint(UdpIPv4Address.Parse(CurrentGameSession.IpAddress), (ushort)CurrentGameSession.Port);
            BoltMatchmaking.JoinSession(CurrentGameSession.GameSessionId, token);
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

    public override void Disconnected(BoltConnection connection)
    {
        base.Disconnected(connection);
        BoltLauncher.Shutdown();
    }

    #endregion

    #region GameLift Game Sessions

    /// <summary>
    /// Retrieves the game session object for a game session ID. Returns null if no game session could be found with the requested game session ID.
    /// </summary>
    /// <param name="gameSessionId"></param>
    /// <returns></returns>
    public GameSession GetCurrentGameSession(string gameSessionId)
    {
        var describeGameSessionsRequest = new DescribeGameSessionsRequest()
        {
            GameSessionId = gameSessionId
        };

        DescribeGameSessionsResponse describeGameSessionsResponse;
        try
        {
            Debug.LogFormat("Looking for game session {0}", gameSessionId);
            describeGameSessionsResponse = ClientInstance.DescribeGameSessions(describeGameSessionsRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return null;
        }

        if (describeGameSessionsResponse.GameSessions.Count > 0)
        {
            return describeGameSessionsResponse.GameSessions[0];
        }
        else
        {
            Debug.LogWarning("Describe game sessions response game sessions list is empty!");
            return null;
        }
    }

    /// <summary>
    /// Creates a GameLift player session, which lets GameLift keep track of players in game sessions.
    /// </summary>
    /// <param name="selectedGameSession"></param>
    /// <param name="joinImmediately"></param>
    public void CreatePlayerSession(GameSession selectedGameSession, bool joinImmediately)
    {
        if (selectedGameSession == null)
        {
            Debug.LogWarning("Selected game session is null!");
            return;
        }

        Debug.LogFormat("{0}/{1} players in selected game session.", selectedGameSession.CurrentPlayerSessionCount, selectedGameSession.MaximumPlayerSessionCount);
        CurrentGameSession = selectedGameSession;

        var createPlayerSessionRequest = new CreatePlayerSessionRequest
        {
            GameSessionId = selectedGameSession.GameSessionId,
            PlayerData = Launcher.Username,
            PlayerId = Launcher.UserID
        };
        Debug.LogFormat("Created player session request for user {0}, username {1}", createPlayerSessionRequest.PlayerId, createPlayerSessionRequest.PlayerData);

        CreatePlayerSessionResponse createPlayerSessionResponse;
        try
        {
            createPlayerSessionResponse = ClientInstance.CreatePlayerSession(createPlayerSessionRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return;
        }

        if (createPlayerSessionResponse == null)
        {
            Debug.LogWarningFormat("Unable to create session for player {0} with game session {1}", Launcher.UserID, selectedGameSession.GameSessionId);
        }
        else
        {
            CurrentPlayerSession = createPlayerSessionResponse.PlayerSession;
            Debug.LogFormat("Successfully created player session {0} with game session {1}", CurrentPlayerSession.PlayerSessionId, selectedGameSession.GameSessionId);
            if (joinImmediately)
            {
                BoltLauncher.StartClient();
                Debug.Log("Started Bolt client!");
            }
        }
    }

    /// <summary>
    /// Directly calls GameLift to create a game session. Should not be used for matchmaking.
    /// </summary>
    /// <param name="maxPlayers"></param>
    /// <param name="sessionToken"></param>
    /// <param name="gameProperties"></param>
    /// <param name="joinImmediately"></param>
    private void CreateGameSession(int maxPlayers, string sessionToken, List<GameProperty> gameProperties, bool joinImmediately)
    {
        var createGameSessionRequest = new CreateGameSessionRequest
        {
            IdempotencyToken = sessionToken,
            MaximumPlayerSessionCount = maxPlayers,
            GameProperties = gameProperties,
            FleetId = FLEET_ID
        };

        CreateGameSessionResponse createGameSessionResponse;
        try
        {
            createGameSessionResponse = ClientInstance.CreateGameSession(createGameSessionRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return;
        }

        if (createGameSessionResponse == null)
        {
            Debug.LogError("Could not create game session!");
            Debug.LogError(createGameSessionResponse.ResponseMetadata.ToString());
        }
        else
        {
            Debug.LogFormat("Successfully created game session {0}", createGameSessionResponse.GameSession.GameSessionId);
            if (joinImmediately)
            {
                CurrentGameSession = createGameSessionResponse.GameSession;
                CreatePlayerSession(CurrentGameSession, true);
            }
        }
    }

    /// <summary>
    /// Finds and joins the first available game session based on filterQuery and sortQuery. Should not be used for matchmaking.
    /// </summary>
    /// <param name="filterQuery">String containing the search criteria for the session search. 
    /// If no filter expression is included, the request returns results for all game sessions in the fleet that are in ACTIVE status.</param>
    /// <param name="sortQuery">Instructions on how to sort the search results. 
    /// If no sort expression is included, the request returns results in random order.</param>
    private void FindGameSession(string filterQuery, string sortQuery, bool joinImmediately)
    {
        var searchGameSessionsRequest = new SearchGameSessionsRequest
        {
            FleetId = FLEET_ID,
            FilterExpression = filterQuery,
            SortExpression = sortQuery,
            Limit = 1
        };

        SearchGameSessionsResponse searchGameSessionsResponse;
        try
        {
            searchGameSessionsResponse = ClientInstance.SearchGameSessions(searchGameSessionsRequest);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            return;
        }

        if (searchGameSessionsResponse == null)
        {
            Debug.LogErrorFormat("Could not find any game session with the following parameters: {0}", filterQuery);
        }
        else
        {
            Debug.LogFormat("Successfully found game session with the following parameters: {0}", filterQuery);
            if (joinImmediately)
            {
                CreatePlayerSession(searchGameSessionsResponse.GameSessions[0], true);
            }
        }
    }

    #endregion

}
