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

    public bool Hitscan => velocity == 0;

    public void Init(int frame, Vector3 position, Vector3 target) {
        state.SpawnFrame = frame;
        state.Origin = position;

        transform.LookAt(target);
        Update();
    }

    void FixedUpdate() {
        if (state.SpawnFrame + lifetime < ServerFrame) {
            gameObject.SetActive(false);
        }

        // TODO: Resolve collisions
    }

    void Update() {
        float dt = (ServerFrame - state.SpawnFrame) / (float)FramesPerSecond;
        transform.position = state.Origin + transform.forward * velocity * dt;
    }
}
