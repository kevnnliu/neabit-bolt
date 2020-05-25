using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Realtime;
using UdpKit.Platform;

public class OculusAuth : MonoBehaviour
{

    public GameObject GameLiftClientPrefab;

    private string oculusId;
    private string oculusNickname;

    private void Start()
    {
        Core.AsyncInitialize().OnComplete(OnInitializationCallback);
    }

    private void OnInitializationCallback(Message<PlatformInitialize> msg)
    {
        if (msg.IsError)
        {
            Debug.LogErrorFormat("Oculus: Error during initialization. Error Message: {0}",
                msg.GetError().Message);
        }
        else
        {
            Debug.Log("Oculus: Initialization successful.");
            Entitlements.IsUserEntitledToApplication().OnComplete(OnIsEntitledCallback);
        }
    }

    private void OnIsEntitledCallback(Message msg)
    {
        if (msg.IsError)
        {
            Debug.LogErrorFormat("Oculus: Error verifying the user is entitled to the application. Error Message: {0}",
                msg.GetError().Message);
        }
        else
        {
            Debug.LogFormat("Oculus: Verified the user is entitled to the application.");
            GetLoggedInUser();
        }
    }

    private void GetLoggedInUser()
    {
        Users.GetLoggedInUser().OnComplete(OnLoggedInUserCallback);
    }

    private void OnLoggedInUserCallback(Message<User> msg)
    {
        if (msg.IsError)
        {
            Debug.LogErrorFormat("Oculus: Error getting logged in user. Error Message: {0}",
                msg.GetError().Message);
        }
        else
        {
            oculusId = msg.Data.ID.ToString(); // do not use msg.Data.OculusID;
            oculusNickname = msg.Data.OculusID.ToString();
            Debug.LogFormat("Oculus: Got player with ID: {0} and Nickname: {1}", oculusId, oculusNickname);
            GetUserProof();
        }
    }

    private void GetUserProof()
    {
        Users.GetUserProof().OnComplete(OnUserProofCallback);
    }

    private void OnUserProofCallback(Message<UserProof> msg)
    {
        if (msg.IsError)
        {
            Debug.LogErrorFormat("Oculus: Error getting user proof. Error Message: {0}",
                msg.GetError().Message);
        }
        else
        {
            string oculusNonce = msg.Data.Value;
            Debug.Log("Oculus: Got user proof.");

            var auth = new AuthenticationValues
            {
                UserId = oculusId,
                AuthType = CustomAuthenticationType.Oculus
            };

            auth.AddAuthParameter("userid", oculusId);
            auth.AddAuthParameter("nonce", oculusNonce);
            auth.AddAuthParameter("version", BoltNetwork.CurrentVersion);

            var platform = new PhotonPlatform(new PhotonPlatformConfig()
            {
                AuthenticationValues = auth
            });

            BoltLauncher.SetUdpPlatform(platform);

            Launcher.UserID = oculusId;
            Launcher.Username = oculusNickname;

            Debug.Log("Oculus: Login completed successfully!");

            Instantiate(GameLiftClientPrefab);
            Debug.Log("Created GameLiftClient!");
            Destroy(this.gameObject);
        }
    }
}
