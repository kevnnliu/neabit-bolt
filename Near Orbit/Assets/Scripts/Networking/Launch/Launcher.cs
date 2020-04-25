using System.Collections.Generic;
using UnityEngine;
using UdpKit;
using Bolt.Matchmaking;
using Bolt.Photon;

public class Launcher : Bolt.GlobalEventListener
{

    public static string Username;
    public static string UserID;
    public static Dictionary<string, object> UserData;

    void Awake()
    {
        Application.targetFrameRate = 60;
        BoltLauncher.StartClient();
    }

    public override void BoltStartBegin()
    {
        // Register any Protocol Token that are you using
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
    }

    public override void BoltStartDone()
    {
        var meta = BoltMatchmaking.CurrentMetadata;

        // Read all custom data sent from your auth server
        object customData;
        if (meta.TryGetValue("Data", out customData))
        {
            var text = "";
            UserData = (Dictionary<string, object>)customData;

            foreach (var item in UserData)
            {
                text += string.Format("{0} : {1}\n", item.Key, item.Value);
            }

            BoltLog.Info(text);
        }

        // Read the UserId of the local player
        if (meta.TryGetValue("UserId", out object userID))
        {
            UserID = (string)userID;
            BoltLog.Info("UserID: {0}", UserID);
        }

        // Read the Nickname of the local player
        if (meta.TryGetValue("Nickname", out object nickName))
        {
            Username = (string)nickName;
            BoltLog.Info("Nickname: {0}", Username);
        }

        // Your usual BoltStartDone behaviour: setup game server or join a session
    }

    public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
    {
        Debug.LogErrorFormat("BoltStartFailed. Reason: {0}", disconnectReason);
    }
}
