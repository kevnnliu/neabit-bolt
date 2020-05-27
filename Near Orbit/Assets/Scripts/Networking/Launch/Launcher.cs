using System.Collections.Generic;
using UnityEngine;
using UdpKit;
using Bolt.Matchmaking;
using Bolt.Photon;
using Amazon.GameLift.Model;

/// <summary>
/// Container class for player info.
/// </summary>
public static class Launcher
{

    public static string Username = "";
    public static string UserID = "";

    /// <summary>
    /// Arbitrary user data.
    /// </summary>
    public static Dictionary<string, object> UserData = new Dictionary<string, object>();

}
