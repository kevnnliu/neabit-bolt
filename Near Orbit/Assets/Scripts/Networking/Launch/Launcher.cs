using System.Collections.Generic;
using UnityEngine;
using UdpKit;
using Bolt.Matchmaking;
using Bolt.Photon;

public class Launcher : Bolt.GlobalEventListener
{

    public GameObject GameLiftClientPrefab;

    public static string Username;
    public static string UserID;
    public static Dictionary<string, object> UserData;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Instantiate(GameLiftClientPrefab);
    }

    public override void BoltStartBegin()
    {
        // Register any Protocol Token that are you using
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        BoltNetwork.RegisterTokenClass<ClientToken>();
        BoltNetwork.RegisterTokenClass<ProjectileToken>();
    }

    public override void BoltStartDone()
    {
        var meta = BoltMatchmaking.CurrentMetadata;

        // Read all custom data sent from your auth server
        if (meta.TryGetValue("Data", out object customData))
        {
            var text = "";
            UserData = (Dictionary<string, object>)customData;

            foreach (var item in UserData)
            {
                text += string.Format("{0} : {1}\n", item.Key, item.Value);
            }

            BoltLog.Info(text);
        }
    }

    public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
    {
        Debug.LogErrorFormat("BoltStartFailed. Reason: {0}", disconnectReason);
    }
}
