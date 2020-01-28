using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviour {

    private string oculusID;
    private string oculusNickname;
    private string oculusNonce;

    private void Start() {
        Core.AsyncInitialize().OnComplete(OnInitializationCallback);
    }

    private void OnInitializationCallback(Message<PlatformInitialize> msg) {
        if (msg.IsError) {
            Debug.LogErrorFormat("Oculus: Error during initialization. Error Message: {0}", msg.GetError().Message);
        }
        else {
            Entitlements.IsUserEntitledToApplication().OnComplete(OnIsEntitledCallback);
        }
    }

    private void OnIsEntitledCallback(Message msg) {
        if (msg.IsError) {
            Debug.LogErrorFormat("Oculus: Error verifying the user is entitled to the application. Error Message: {0}", msg.GetError().Message);
        }
        else {
            GetLoggedInUser();
        }
    }

    private void GetLoggedInUser() {
        Users.GetLoggedInUser().OnComplete(OnLoggedInUserCallback);
    }

    private void OnLoggedInUserCallback(Message<User> msg) {
        if (msg.IsError) {
            Debug.LogErrorFormat("Oculus: Error getting logged in user. Error Message: {0}", msg.GetError().Message);
        }
        else {
            oculusID = msg.Data.ID.ToString(); // do not use msg.Data.OculusID;
            oculusNickname = msg.Data.OculusID.ToString();
            GetUserProof();
        }
    }

    private void GetUserProof() {
        Users.GetUserProof().OnComplete(OnUserProofCallback);
    }

    private void OnUserProofCallback(Message<UserProof> msg) {
        if (msg.IsError) {
            Debug.LogErrorFormat("Oculus: Error getting user proof. Error Message: {0}", msg.GetError().Message);
        }
        else{
            oculusNonce = msg.Data.Value;
            ConnectToPhoton();
        }
    }

    private void ConnectToPhoton() {
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = oculusID;
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Oculus;
        PhotonNetwork.AuthValues.AddAuthParameter("userid", oculusID);
        PhotonNetwork.AuthValues.AddAuthParameter("nonce", oculusNonce);
        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.NickName = oculusNickname;
    }

}
