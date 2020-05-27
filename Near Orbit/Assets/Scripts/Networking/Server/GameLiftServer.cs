using System.Collections.Generic;
using UnityEngine;
using Aws.GameLift;
using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;
using UdpKit;
using Bolt;
using Bolt.Photon;
using Bolt.Matchmaking;
using UdpKit.Platform;
using System.Collections;
using System.Net.NetworkInformation;
using System.Net;
using System.Linq;

public class GameLiftServer : GlobalEventListener
{
    public GameSession ServerSession;

    private const float SERVER_TIMEOUT = 300f;

    private bool StartedGameLift = false;
    private float ShutdownTimer = SERVER_TIMEOUT;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        //Set the port that your game service is listening on for incoming player connections (hard-coded default)
        int listeningPort = 7777;
        for (int a = 7777; a < 7799; a++)
        {
            if (PortInUse(a) == false)
            {
                listeningPort = a;
                break;
            }
        }
        //int listeningPort = int.Parse(GetArg("-p", "-port") ?? "7777");

        StartGameLiftServer(listeningPort);
    }

    private void Update()
    {
        if (BoltNetwork.Clients.Count() == 0)
        {
            ShutdownTimer -= Time.deltaTime;
            if (ShutdownTimer <= 0)
            {
                GameLiftServerAPI.TerminateGameSession();
            }
        }
        else
        {
            ShutdownTimer = SERVER_TIMEOUT;
        }
    }

    void OnApplicationQuit()
    {
        //Make sure to call GameLiftServerAPI.Destroy() when the application quits. 
        //This resets the local connection with GameLift's agent.
        if (StartedGameLift)
        {
            GameLiftServerAPI.Destroy();
            LogToConsoleDispatcher("Application quit! GameLiftServer instance destroyed!");
        }

        LogToConsoleDispatcher("Application quit! Shutting down Bolt!");
        BoltNetwork.Shutdown();
    }

    //This makes game server processes go active on Amazon GameLift
    public void StartGameLiftServer(int listeningPort)
    {
        Debug.LogFormat("Starting GameLiftServer with Bolt server on port {0}", listeningPort);
        StartedGameLift = true;

        //InitSDK establishes a local connection with the Amazon GameLift agent to enable 
        //further communication.
        var initSDKOutcome = GameLiftServerAPI.InitSDK();
        if (initSDKOutcome.Success)
        {
            ProcessParameters processParameters = new ProcessParameters(
                (gameSession) =>
                {
                    //Respond to new game session activation request. GameLift sends activation request
                    //to the game server along with a game session object containing game properties
                    //and other settings. Once the game server is ready to receive player connections, 
                    //invoke GameLiftServerAPI.ActivateGameSession()
                    GameLiftServerAPI.ActivateGameSession();

                    ServerSession = gameSession;

                    UnityMainThreadDispatcher.Instance().Enqueue(StartBoltServerTask(listeningPort));
                    LogToConsoleDispatcher(string.Format("Enqueued StartBoltServerTask for game session {0}", ServerSession.GameSessionId));
                },
                (updateGameSession) => {
                    //When a game session is updated (e.g. by FlexMatch backfill), GameLiftsends a request to the game
                    //server containing the updated game session object.  The game server can then examine the provided
                    //matchmakerData and handle new incoming players appropriately.
                    //updateReason is the reason this update is being supplied.
                },
                () =>
                {
                    //OnProcessTerminate callback. GameLift invokes this callback before shutting down 
                    //an instance hosting this game server. It gives this game server a chance to save
                    //its state, communicate with services, etc., before being shut down. 
                    //In this case, we simply tell GameLift we are indeed going to shut down.
                    GameLiftServerAPI.ProcessEnding();

                    LogToConsoleDispatcher("GameLift OnProcessTerminate! Shutting down Bolt!");
                    BoltNetwork.Shutdown();
                },
                () =>
                {
                    //This is the HealthCheck callback.
                    //GameLift invokes this callback every 60 seconds or so.
                    //Here, a game server might want to check the health of dependencies and such.
                    //Simply return true if healthy, false otherwise.
                    //The game server has 60 seconds to respond with its health status. 
                    //GameLift will default to 'false' if the game server doesn't respond in time.
                    //In this case, we're always healthy!
                    return true;
                },
                //Here, the game server tells GameLift what port it is listening on for incoming player 
                //connections. In this example, the port is hardcoded for simplicity. Active game
                //that are on the same instance must have unique ports.
                listeningPort,
                new LogParameters(new List<string>()
                {
                    //Here, the game server tells GameLift what set of files to upload when the game session ends.
                    //GameLift uploads everything specified here for the developers to fetch later.
                    "C:\\game\\logs"
                }));

            //Calling ProcessReady tells GameLift this game server is ready to receive incoming game sessions!
            var processReadyOutcome = GameLiftServerAPI.ProcessReady(processParameters);
            if (processReadyOutcome.Success)
            {
                LogToConsoleDispatcher("ProcessReady success, GameLift ready to host game sessions");
            }
            else
            {
                Debug.LogErrorFormat("ProcessReady failure: {0}", processReadyOutcome.Error.ToString());
                Application.Quit();
            }
        }
        else
        {
            Debug.LogErrorFormat("InitSDK failure: {0}", initSDKOutcome.Error.ToString());
            Application.Quit();
        }
    }

    public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
    {
        ClientToken clientToken = (ClientToken)token;
        LogToConsoleDispatcher(string.Format("Received client token for player session {0}", clientToken.PlayerSessionId));

        //Ask GameLift to verify sessionID is valid, it will change player slot from "RESERVED" to "ACTIVE"
        GenericOutcome outcome = GameLiftServerAPI.AcceptPlayerSession(clientToken.PlayerSessionId);
        if (outcome.Success)
        {
            GameLiftServerAPI.AcceptPlayerSession(clientToken.PlayerSessionId);
            BoltNetwork.Accept(endpoint);
            LogToConsoleDispatcher("Connect request accepted");
        }
        else
        {
            BoltNetwork.Refuse(endpoint);
            LogToConsoleDispatcher("Connect request refused");
        }

        /*
        This data type is used to specify which player session(s) to retrieve. 
        It can be used in several ways: 
        (1) provide a PlayerSessionId to request a specific player session; 
        (2) provide a GameSessionId to request all player sessions in the specified game session; or
        (3) provide a PlayerId to request all player sessions for the specified player.
        For large collections of player sessions, use the pagination parameters to retrieve results as sequential pages.
        */
        DescribePlayerSessionsRequest sessions = new DescribePlayerSessionsRequest()
        {
            PlayerSessionId = clientToken.PlayerSessionId,
            GameSessionId = ServerSession.GameSessionId,
            PlayerId = clientToken.UserId
        };

        DescribePlayerSessionsOutcome sessionsOutcome = GameLiftServerAPI.DescribePlayerSessions(sessions);
        string playerId = sessionsOutcome.Result.PlayerSessions[0].PlayerId;
        LogToConsoleDispatcher(string.Format("Connect request for player ID {0}", playerId));
    }

    #region Bolt Callbacks

    public override void BoltStartBegin()
    {
        // Register any IProtocolToken that you are using
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        BoltNetwork.RegisterTokenClass<ClientToken>();
        BoltNetwork.RegisterTokenClass<ProjectileToken>();

        LogToConsoleDispatcher("Finished registering custom token classes");
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            string sessionId = ServerSession.GameSessionId;
            //If GameSessionId was not set, throw an error and shut down the BoltNetwork
            if (sessionId.Length == 0)
            {
                LogToConsoleDispatcher("GameSessionId not set! Shutting down Bolt.");
                BoltNetwork.Shutdown();
                return;
            }

            List<GameProperty> gameProperties = ServerSession.GameProperties;
            PhotonRoomProperties roomProperties = new PhotonRoomProperties
            {
                IsOpen = true,
                IsVisible = true
            };

            foreach (GameProperty gameProperty in gameProperties)
            {
                roomProperties.AddRoomProperty(gameProperty.Key, gameProperty.Value);
            }

            string requestedMap = (string)roomProperties.CustomRoomProperties["m"];

            // Create the Bolt session
            BoltMatchmaking.CreateSession(sessionId, roomProperties, requestedMap);
            LogToConsoleDispatcher(string.Format("Created Bolt session {0} and map {1}", sessionId, requestedMap));
        }
        else
        {
            LogToConsoleDispatcher("Attempting to create game not as server! Shutting down Bolt!");
            BoltNetwork.Shutdown();
        }
    }

    public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {
            ClientToken myToken = (ClientToken)connection.ConnectToken;
            connection.UserData = myToken.PlayerSessionId;
            LogToConsoleDispatcher(string.Format("Player session {0} connected!", (string)connection.UserData));
        }
    }

    public override void Disconnected(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {
            GameLiftServerAPI.RemovePlayerSession((string)connection.UserData);
            LogToConsoleDispatcher(string.Format("Player session {0} disconnected!", (string)connection.UserData));
        }
    }

    #endregion

    #region Utility Functions

    private IEnumerator StartBoltServerTask(int listeningPort)
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());
        BoltLauncher.StartServer(listeningPort);
        LogToConsoleDispatcher("Started Bolt server!");

        yield return null;
    }

    private void LogToConsoleDispatcher(string logText)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.Log(logText));
    }

    private bool PortInUse(int port)
    {
        bool inUse = false;

        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] ipEndPoints = ipProperties.GetActiveUdpListeners();

        foreach (IPEndPoint endPoint in ipEndPoints)
        {
            if (endPoint.Port == port)
            {
                inUse = true;
                break;
            }
        }
        return inUse;
    }

    #endregion
}
