using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BaseShip : MonoBehaviour {

    #region Serialized Fields

    [SerializeField]
    private float thrust;
    [SerializeField]
    private float rollRate;
    [SerializeField]
    private float yawRate;
    [SerializeField]
    private float pitchRate;
    [SerializeField]
    private int totalModCapacity;
    [SerializeField]
    private float baseHealth;
    [SerializeField]
    private float baseEnergy;
    [SerializeField]
    private float energyChargeRate;
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
    private float energy;
    private bool invincible;

    private IMoveInput moveInput;
    private Movement movement;
    private ModBox weapons;
    private ModBox specials;

    void Awake() {
        SetupCamera();
        LoadBaseShip();
    }

    void Update() {
        invincible = false; // TODO: Check if in safe zone, if yes then invincible = true
        if (ProcessModActivation() && energy < baseEnergy) {
            float charge = energyChargeRate * Time.deltaTime;
            energy = energy + charge < baseEnergy ? energy + charge : baseEnergy;
        }
        ConvertInputs(moveInput);
        ApplyMovement(movement);
    }

    public Movement GetMovement() {
        return movement;
    }

    public IMoveInput GetMoveInput() {
        return moveInput;
    }

    public void SetInvincibility(bool enabled) {
        invincible = enabled;
    }

    /// <summary>
    /// Returns whether or not the ship can activate a mod, and if so subtracts the energy required.
    /// </summary>
    public bool SpendEnergy(IShipMod mod) {
        if (mod.TypeOfMod == ModType.Weapon && !((BaseWeapon) mod).ReadyToFire()) {
            return false;
        }
        float cost = mod.IsContinuous ? mod.EnergyCost * Time.deltaTime : mod.EnergyCost;
        if (energy >= cost) {
            energy -= cost;
            return true;
        }
        return false;
    }

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
        return moveInput.GetAimPoint();
    }

    /// <summary>
    /// Process inputs and activate mods using the ModBox instances, returns true if no mods were activated.
    /// </summary>
    private bool ProcessModActivation() {
        bool didActivateSpecial = specials.ActivateMod(this, moveInput.SpecialActivated());
        bool didActivateWeapon = weapons.ActivateMod(this, moveInput.WeaponActivated());
        return !didActivateWeapon && !didActivateSpecial;
    }

    /// <summary>
    /// Initializes control, movement, and module instances.
    /// </summary>
    private void LoadBaseShip() {
        health = baseHealth;
        energy = baseEnergy;

        movement = new Movement(rollRate, yawRate, pitchRate, thrust, transform);
        
        // TODO: Load ModBox instances (CURRENTLY HARD CODED)
        weapons = new ModBox(weaponMounts, new Module[] {Module.LaserGun});
        specials = new ModBox(specialMounts, new Module[] {Module.Boost, Module.Shield});
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
    /// the corresponding camera object.
    /// </summary>
    private void SetupCamera() {
        if (XRSettings.isDeviceActive) {
            moveInput = new GestureInput(transform);
        }
        else {
            XRSettings.enabled = false;
            moveInput = new KeyboardInput(transform);
        }
    }

}
