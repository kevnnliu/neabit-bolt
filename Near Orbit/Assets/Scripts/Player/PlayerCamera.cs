using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : BoltSingletonPrefab<PlayerCamera>
{

    public void SetTarget(BoltEntity entity)
    {
        transform.position = entity.transform.position + new Vector3(0f, 1.5f, 0f);
        transform.parent = entity.transform;
    }

    public Transform GetTrackingSpace()
    {
        return transform.Find("TrackingSpace");
    }

}
