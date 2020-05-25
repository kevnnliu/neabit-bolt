using UnityEngine;

public class PlayerObject
{

    public BoltEntity character;
    public BoltConnection connection;

    public bool IsServer => connection == null;

    public bool IsClient => connection != null;

    public void Spawn()
    {

        if (!character)
        {
            BoltLog.Warn("Spawning ship from " + (IsServer ? "server" : "client"));
            character = BoltNetwork.Instantiate(BoltPrefabs.ViperPrefab, MapInfo.SpawnLocations[0], Quaternion.identity);

            if (IsServer)
            {
                character.TakeControl();
            }
            else
            {
                character.AssignControl(connection);
            }
        }

        character.transform.position = MapInfo.SpawnLocations[0];
    }
}
