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
        Debug.Log("Attached");
    }

    public override void SimulateController() {
        if (input.ReadInputs) {
            input.UpdateInput();
            IShipMoveCommandInput moveCommandInput = ShipMoveCommand.Create();

            moveCommandInput.Thrust = input.GetThrustInput();
            moveCommandInput.Rotation = input.GetRotationInput();
            moveCommandInput.ReticlePoint = input.GetReticlePoint();

            entity.QueueInput(moveCommandInput);
            Debug.Log("Inputs queued over network");
        }
    }

    public override void ExecuteCommand(Command command, bool resetState) {
        ShipMoveCommand moveCommand = (ShipMoveCommand) command;

        if (resetState) {
            movement.SetState(moveCommand.Result.Position, moveCommand.Result.Rotation);
            ApplyMovement();

            input.SetAimPoint(moveCommand.Result.AimPoint);
            Debug.Log("Reset state");
        } 
        else {
            movement.ComputeNewTransform(transform, moveCommand.Input.Rotation, moveCommand.Input.Thrust);
            ApplyMovement();

            input.ComputeAimPoint(moveCommand.Input.ReticlePoint);

            moveCommand.Result.Position = movement.GetNewPosition();
            moveCommand.Result.Rotation = movement.GetNewRotation();
            moveCommand.Result.AimPoint = input.GetAimPoint();
            Debug.Log("Processed inputs");
        }
    }

    #endregion

    void Awake() {
        SetupControl();
        LoadBaseShip();
    }

    void Update() {
        invincible = false; // TODO: Check if in safe zone, if yes then invincible = true
        
        // ConvertInputs(input);
        // ApplyMovement(movement);
        input.UpdateInput();
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
    private void ConvertInputs() {
        if (input.ReadInputs) {
            input.UpdateInput();
            movement.ComputeNewTransform(transform, input.GetRotationInput(), input.GetThrustInput());
        }
    }

    /// <summary>
    /// Applies the Movement instance by updating the ship's Transform component.
    /// </summary>
    private void ApplyMovement() {
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
