using UnityEngine;
using Bolt;
using static BoltNetwork;

public class Projectile : EntityBehaviour<IProjectileState> {
    [SerializeField]
    private float damage;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private int lifetime;

    public override void Attached() {
        var token = (ProjectileToken)entity.AttachToken;
        state.SpawnFrame = token.SpawnFrame;
        state.Origin = transform.position;
        transform.rotation = transform.rotation;
        Update();
    }

    void FixedUpdate() {
        if (IsServer && state.SpawnFrame + lifetime < ServerFrame) {
            BoltNetwork.Destroy(gameObject);
        }

        // TODO: Resolve collisions
    }

    void Update() {
        float dt = (ServerFrame - state.SpawnFrame) / (float)FramesPerSecond;
        transform.position = state.Origin + transform.forward * velocity * dt;
    }
}
