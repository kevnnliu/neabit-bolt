using System;
using Bolt.Photon;
using Bolt.Matchmaking;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// OBSOLETE. Use for reference code.
/// </summary>
public class ServerManager : Bolt.GlobalEventListener
{

    //public string Map = "";
    //public string Gamemode = "";
    //public string RoomID = "";
    //public GameObject GameLiftServerPrefab;

    //// Use this for initialization
    //void Start()
    //{
    //    // Get custom arguments from command line
    //    Map = GetArg("-m", "-map") ?? Map;
    //    Gamemode = GetArg("-g", "-gamemode") ?? Gamemode; // ex: get game type from command line
    //    RoomID = GetArg("-r", "-room") ?? RoomID;

    //    // Validate the requested Level
    //    var validMap = false;

    //    foreach (string value in BoltScenes.AllScenes)
    //    {
    //        if (SceneManager.GetActiveScene().name != value)
    //        {
    //            if (Map == value)
    //            {
    //                validMap = true;
    //                break;
    //            }
    //        }
    //    }

    //    if (!validMap)
    //    {
    //        Debug.LogError("Invalid configuration: please verify level name");
    //        Application.Quit();
    //    }

    //    // Start the Server
    //    BoltLauncher.StartServer();
    //}

    //public override void BoltStartBegin()
    //{
    //    // Register any Protocol Token that are you using
    //    BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
    //}

    //public override void BoltStartDone()
    //{
    //    if (BoltNetwork.IsServer)
    //    {
    //        // Create some room custom properties
    //        PhotonRoomProperties roomProperties = new PhotonRoomProperties();

    //        roomProperties.AddRoomProperty("g", Gamemode); // ex: game type
    //        roomProperties.AddRoomProperty("m", Map); // ex: map id

    //        roomProperties.IsOpen = true;
    //        roomProperties.IsVisible = true;

    //        //// If RoomID was not set, create a random one
    //        //if (RoomID.Length == 0)
    //        //{
    //        //    RoomID = Guid.NewGuid().ToString();
    //        //}
    //        // If RoomID was not set, throw an error and shut down the server
    //        if (RoomID.Length == 0)
    //        {
    //            Debug.LogError("RoomID not set! Shutting down.");
    //            Application.Quit();
    //        }

    //        // Create the Photon Room
    //        BoltMatchmaking.CreateSession(RoomID, roomProperties);

    //        // Load the requested Level
    //        BoltNetwork.LoadScene(Map);
    //    }
    //}

    ///// <summary>
    ///// Utility function to detect if the game instance was started in headless mode.
    ///// </summary>
    ///// <returns><c>true</c>, if headless mode was used, <c>false</c> otherwise.</returns>
    //public static bool IsHeadlessMode()
    //{
    //    return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
    //}

    //static string GetArg(params string[] names)
    //{
    //    var args = Environment.GetCommandLineArgs();
    //    for (int i = 0; i < args.Length; i++)
    //    {
    //        foreach (var name in names)
    //        {
    //            if (args[i] == name && args.Length > i + 1)
    //            {
    //                return args[i + 1];
    //            }
    //        }
    //    }

    //    return null;
    //}
}