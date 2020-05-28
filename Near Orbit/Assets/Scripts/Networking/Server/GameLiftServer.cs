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
    public GameSession ServerSession = null;

    private bool GameLiftServerSDKReady = false;
    private ProcessParameters ServerProcessParameters;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        StartGameLiftServer();
    }

    private void OnApplicationQuit()
    {
        LogToConsoleDispatcher("Application quit!");

        BoltShutdownHandler();

        //Make sure to call GameLiftServerAPI.Destroy() when the application quits. 
        //This resets the local connection with GameLift's agent.
        if (GameLiftServerSDKReady)
        {
            LogToConsoleDispatcher("Application quit! Shutting down GameLiftServer!");
            GameLiftServerAPI.Destroy();
        }
    }

    //This makes game server processes go active on Amazon GameLift
    public void StartGameLiftServer()
    {
        //Set the port for incoming player connections to Bolt (hard-coded default)
        int listeningPort = 7777;
        for (int p = 7777; p < 7799; p++)
        {
            if (PortInUse(p) == false)
            {
                listeningPort = p;
                break;
            }
        }

        Debug.LogFormat("Starting server on port {0}", listeningPort);

        //InitSDK establishes a local connection with the Amazon GameLift agent to enable 
        //further communication.
        var initSDKOutcome = GameLiftServerAPI.InitSDK();
        if (initSDKOutcome.Success)
        {
            GameLiftServerSDKReady = true;
            ServerProcessParameters = new ProcessParameters(
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
                (updateGameSession) =>
                {
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
                    LogToConsoleDispatcher("GameLift OnProcessTerminate! Shutting down Bolt!");
                    BoltShutdownHandler();

                    TerminateServerProcess();
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
                    "C:\\game\\logs\\ServerLog.txt"
                }));

            //Calling ProcessReady tells GameLift this game server is ready to receive incoming game sessions!
            var processReadyOutcome = GameLiftServerAPI.ProcessReady(ServerProcessParameters);
            if (processReadyOutcome.Success)
            {
                LogToConsoleDispatcher("ProcessReady success, GameLift ready to host game sessions");
            }
            else
            {
                LogToConsoleDispatcher(string.Format("ProcessReady failure: {0}", processReadyOutcome.Error.ToString()));
                PrepareForNewGameSession();
            }
        }
        else
        {
            LogToConsoleDispatcher(string.Format("InitSDK failure: {0}", initSDKOutcome.Error.ToString()));
            TerminateServerProcess();
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
            //If ServerSession.GameSessionId was not set, reset and prepare for a new game session
            if (sessionId.Length == 0)
            {
                LogToConsoleDispatcher("GameSessionId not set!");
                PrepareForNewGameSession();
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

            //Create the Bolt session
            BoltMatchmaking.CreateSession(sessionId, roomProperties, requestedMap);
            LogToConsoleDispatcher(string.Format("Created Bolt session {0} and map {1}", sessionId, requestedMap));
        }
        else
        {
            LogToConsoleDispatcher("Attempting to create game not as server!");
            PrepareForNewGameSession();
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

    /// <summary>
    /// Shuts down Bolt if an instance is running.
    /// </summary>
    private void BoltShutdownHandler()
    {
        if (BoltNetwork.IsRunning)
        {
            LogToConsoleDispatcher("Shutting down Bolt!");
            BoltNetwork.Shutdown();
        }
    }

    private void TerminateServerProcess()
    {
        GameLiftServerSDKReady = false;
        var endProcessOutcome = GameLiftServerAPI.ProcessEnding();
        if (endProcessOutcome.Success)
        {
            LogToConsoleDispatcher("Successfully shut down the server process!");
        }
        else
        {
            LogToConsoleDispatcher(string.Format("Error shutting down the server process: {0}, {1}",
                endProcessOutcome.Error.ErrorName,
                endProcessOutcome.Error.ErrorMessage));
            LogToConsoleDispatcher("Quitting the application!");
            Application.Quit();
        }
    }

    private void PrepareForNewGameSession()
    {
        LogToConsoleDispatcher("Preparing for new game session!");

        BoltShutdownHandler();

        var terminateOutcome = GameLiftServerAPI.TerminateGameSession();
        if (terminateOutcome.Success || ServerSession == null)
        {
            ServerSession = null;
            LogToConsoleDispatcher("Successfully terminated game session!");

            if (GameLiftServerSDKReady)
            {
                var processReadyOutcome = GameLiftServerAPI.ProcessReady(ServerProcessParameters);
                if (processReadyOutcome.Success)
                {
                    LogToConsoleDispatcher("Ready for new game session!");
                }
                else
                {
                    LogToConsoleDispatcher(string.Format("Error preparing for new game session: {0}, {1}",
                        processReadyOutcome.Error.ErrorName,
                        processReadyOutcome.Error.ErrorMessage));
                    TerminateServerProcess();
                }
            }
            else
            {
                StartGameLiftServer();
            }
        }
        else
        {
            LogToConsoleDispatcher(string.Format("Error terminating game session: {0}, {1}",
                    terminateOutcome.Error.ErrorName,
                    terminateOutcome.Error.ErrorMessage));
            TerminateServerProcess();
        }
    }

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
