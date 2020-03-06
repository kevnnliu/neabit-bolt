using UnityEngine;

public class PlayerObject {

    public BoltEntity character;
    public BoltConnection connection;


    public bool IsServer {
        get {
            return connection == null;
        }
    }

    public bool IsClient {
        get {
            return connection != null;
        }
    }

    public void Spawn() {
        if (!character) {
            character = BoltNetwork.Instantiate(BoltPrefabs.TestShip, MapInfo.SpawnLocations[0], Quaternion.identity);

            if (IsServer) {
                character.TakeControl();
            }
            else {
                character.AssignControl(connection);
            }
        }

        character.transform.position = MapInfo.SpawnLocations[0];
    }

}
