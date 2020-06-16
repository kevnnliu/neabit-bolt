using UnityEngine;

[System.Obsolete]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private float range;

    //private NetworkId owner;
    private float maxDistance;

    void FixedUpdate()
    {
        //if (!IsServer) return;

        //if (ResolveCollisions(ServerFrame))
        //{
        //    if (IsServer)
        //    {
        //        BoltNetwork.Destroy(gameObject);
        //    }
        //    else
        //    {
        //        // Hides object?
        //        transform.localScale = Vector3.zero;
        //    }
        //}
    }

    /// <summary>
    /// Resolves projectile collisions with static geometry and players.
    /// Returns true if the projectile is to be destroyed.
    /// </summary>
    //protected bool ResolveCollisions(int frame)
    //{
        //float distance = DistanceAtFrame(frame);
        //float nextDistance = Mathf.Min(DistanceAtFrame(frame + 1), maxDistance, range);
        // Static impact
        //if (distance >= maxDistance)
        //{
        //    // TODO: Spawn impact entity
        //    Debug.LogWarning("Hit a wall!");
        //    return true;
        //}
        //using (var hits = RaycastAll(new Ray(PositionAtFrame(frame), transform.forward), frame))
        //{
        //    // Get closest collision
        //    int min = -1;
        //    for (int i = 0; i < hits.count; i++)
        //    {
        //        if (hits[i].body.GetComponent<BaseShip>().entity.NetworkId != owner)
        //        {
        //            if (min == -1 || hits[i].distance < hits[min].distance)
        //            {
        //                min = i;
        //            }
        //        }
        //    }
        //    // Deal damage if collision occurs on this frame
        //    if (min != -1 && hits[min].distance + distance < nextDistance)
        //    {
        //        hits[min].body.GetComponent<BaseShip>().TakeDamage(damage);
        //        // TODO: Spawn impact entity
        //        return true;
        //    }
        //}
        // Despawn at max range
        //return distance >= range;
    //}

    //private float DistanceAtFrame(int frame)
    //{
    //    float dt = (frame - state.SpawnFrame) / (float)FramesPerSecond;
    //    return velocity * dt;
    //}

    //private Vector3 PositionAtFrame(int frame)
    //{
    //    return state.Origin + transform.forward * DistanceAtFrame(frame);
    //}
}
