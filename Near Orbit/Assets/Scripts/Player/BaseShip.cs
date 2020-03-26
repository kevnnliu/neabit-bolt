using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Bolt;

public class BaseShip : EntityBehaviour<IShipState> {

    #region Serialized Fields
    
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

    private List<Weapon> weapons = new List<Weapon>();

    #endregion

    private const float pitchYawBorder = 18f;
    private const float rollBorder = 25f;

    private float health;
    private bool invincible;

    private IMoveInput input;

    //private ModuleGroup modules = new ModuleGroup();

    #region Bolt Functions

    public override void Attached() {
        state.SetTransforms(state.Transform, transform);
        state.EquippedWeapon = 0;

        Debug.Log("Attached");
    }

    public override void SimulateController() {
        if (input.ReadInputs) {
            input.UpdateInput();
            IShipCommandInput moveCommandInput = ShipCommand.Create();

            moveCommandInput.Thrust = input.GetThrustInput();
            moveCommandInput.Rotation = input.GetRotationInput();
            moveCommandInput.ReticlePoint = input.GetReticlePoint();

            moveCommandInput.Fire = input.WeaponActivated();

            entity.QueueInput(moveCommandInput);
            //Debug.Log("Inputs queued over network");
        }
    }

    public override void ExecuteCommand(Command command, bool resetState) {
        ShipCommand moveCommand = (ShipCommand)command;

        if (resetState) {
            Movement.SetState(moveCommand.Result.Position, moveCommand.Result.Rotation);
            ApplyMovement();

            input.SetAimPoint(moveCommand.Result.AimPoint);

            weapons[state.EquippedWeapon].Firing = moveCommand.Result.Firing;
            //Debug.Log("Reset state");
        } 
        else {
            Movement.ComputeNewTransform(transform, moveCommand.Input.Rotation, moveCommand.Input.Thrust);
            ApplyMovement();

            input.ComputeAimPoint(moveCommand.Input.ReticlePoint);

            moveCommand.Result.Position = Movement.GetNewPosition();
            moveCommand.Result.Rotation = Movement.GetNewRotation();
            moveCommand.Result.AimPoint = input.GetAimPoint();

            moveCommand.Result.Firing = moveCommand.Input.Fire;
            weapons[state.EquippedWeapon].Firing = moveCommand.Result.Firing;
            //Debug.Log("Processed inputs");
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
        //modules.Update(input);
    }

    public void AddWeapon(string prefabID) {
        var prefab = Resources.Load<GameObject>(prefabID);
        Weapon weapon = Instantiate(prefab, weaponMounts[weapons.Count].position, weaponMounts[weapons.Count].rotation, transform)
            .GetComponent<Weapon>();
        weapon.Owner = this;
        weapons.Add(weapon);
    }

    public void SetInvincibility(bool enabled) {
        invincible = enabled;
    }

    public Movement Movement { get; private set; }

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

        Movement = new Movement(stats, transform);

        AddWeapon("Weapons/MachineGun");
        AddWeapon("Weapons/LaserGun");

        // TODO: Load ModBox instances (CURRENTLY HARD CODED)
        //modules.AddModule(ModuleManager.CreateModule<BaseWeapon>("Weapons/LaserGun", this, weaponMounts[0]));
        //modules.AddModule(ModuleManager.CreateModule<BaseSpecial>("Specials/Boost", this, specialMounts[0]));
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
            Movement.ComputeNewTransform(transform, input.GetRotationInput(), input.GetThrustInput());
        }
    }

    /// <summary>
    /// Applies the Movement instance by updating the ship's Transform component.
    /// </summary>
    private void ApplyMovement() {
        if (Movement != null) {
            transform.position = Movement.GetNewPosition();
            transform.rotation = Movement.GetNewRotation();
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
