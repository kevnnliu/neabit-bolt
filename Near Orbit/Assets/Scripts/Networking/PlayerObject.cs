using UnityEngine;
using Bolt;

public class PlayerObject {

    public BoltEntity character;
    public BoltConnection connection;

    public bool IsServer => connection == null;

    public bool IsClient => connection != null;

    public void Spawn() {
        if (!character) {
            character = BoltNetwork.Instantiate(BoltPrefabs.ViperShip, MapInfo.SpawnLocations[0], Quaternion.identity);

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
