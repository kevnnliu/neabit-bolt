using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.GameLift;
using Amazon.GameLift.Model;

public class GameLiftClient : Bolt.GlobalEventListener
{

    private AmazonGameLiftConfig ClientConfig;

    public void CreateClientConfiguration()
    {
        ClientConfig = new AmazonGameLiftConfig();
    }

}
