using UnityEngine;
using UnityEngine.XR;
using Bolt;

public class BaseShip : EntityBehaviour<IShipState> {

    #region Serialized Fields
    
    [SerializeField]
    private int totalModCapacity;
    [SerializeField]
    private float baseHealth = 100;
    [SerializeField]
    private ShipStats stats = new ShipStats(45, 30, 35, 12);
    [SerializeField]
    private Transform[] weaponMounts;
    [SerializeField]
    private Transform[] specialMounts;
    [SerializeField]
    private PointAim pointAim;

    #endregion

    private const float pitchYawBorder = 18f;
    private const float rollBorder = 25f;

    private float health;
    private bool invincible;

    private IMoveInput input;
    private Movement movement;
    private ModuleGroup modules = new ModuleGroup();

    #region Bolt Functions

    public override void Attached() {
        state.SetTransforms(state.Transform, transform);
    }

    #endregion

    void Awake() {
        SetupControl();
        LoadBaseShip();
    }

    void Update() {
        invincible = false; // TODO: Check if in safe zone, if yes then invincible = true
        
        //ConvertInputs(input);
        if (input.ReadInputs) {
            input.UpdateInput();
            movement.ComputeNewTransform(transform, input);
        }
        ApplyMovement(movement);
        modules.Update(input);
    }

    public void SetInvincibility(bool enabled) {
        invincible = enabled;
    }

    public Movement Movement => movement;

    /// <summary>
    /// Checks invincibility, subtracts from Health, and initiates respawn when necessary.
    /// </summary>
    public void TakeDamage(float damage) {
        if (!invincible) {
            health -= damage;
            if (health <= 0f) {
                Debug.Log("I died :(");
                Respawn();
            }
        }
    }

    /// <summary>
    /// Called by projectile or beam to show hitmarker on hit.
    /// </summary>
    public void ShowHitMarker() {
        // TODO: Implement showing hitmarker
    }

    /// <summary>
    /// Returns the Vector3 point that is being aimed at.
    /// </summary>
    public Vector3 AimTarget() {
        return input.GetAimPoint();
    }

    /// <summary>
    /// Initializes control, movement, and module instances.
    /// </summary>
    private void LoadBaseShip() {
        health = baseHealth;

        movement = new Movement(stats, transform);

        // TODO: Load ModBox instances (CURRENTLY HARD CODED)
        modules.AddModule(ModuleManager.CreateModule<BaseWeapon>("Weapons/LaserGun", this, weaponMounts[0]));
        modules.AddModule(ModuleManager.CreateModule<BaseSpecial>("Specials/Boost", this, specialMounts[0]));
    }

    private void Respawn() {
        // TODO: Implement networked respawning and reset health/energy
    }

    /// <summary>
    /// Processes an IMoveInput instance and updates the ship's Movement instance.
    /// </summary>
    private void ConvertInputs(IMoveInput moveInput) {
        if (moveInput.ReadInputs) {
            moveInput.UpdateInput();
            movement.ComputeNewTransform(transform, moveInput);
        }
    }

    /// <summary>
    /// Applies the Movement instance by updating the ship's Transform component.
    /// </summary>
    private void ApplyMovement(Movement movement) {
        if (movement != null) {
            transform.position = movement.GetNewPosition();
            transform.rotation = movement.GetNewRotation();
        }
    }

    /// </summary>
    /// Detects whether or not there is a VR device connected and enables
    /// the corresponding control scheme.
    /// </summary>
    private void SetupControl() {
        if (XRSettings.isDeviceActive) {
            input = new GestureInput(transform);
            Debug.Log("Gesture input enabled");
        }
        else {
            input = new KeyboardInput(transform);
            Debug.Log("Keyboard input enabled");
        }
    }

}
