using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShip : MonoBehaviour, IShip {

    #region Serialized Fields

    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _totalModCapacity;
    [SerializeField]
    private float _baseHealth;
    [SerializeField]
    private float _baseEnergy;
    [SerializeField]
    private float _energyChargeRate;
    [SerializeField]
    private float rollRate;
    [SerializeField]
    private float yawRate;
    [SerializeField]
    private float pitchRate;

    #endregion

    protected float _health;
    protected float _energy;
    protected bool _invincible;

    #region Properties

    public float BaseHealth {
        get {
            return _baseHealth;
        }
    }

    public float Health { 
        get {
            return _health;
        } 
        set {
            _health = value;
        } 
    }

    public float BaseEnergy {
        get {
            return _baseEnergy;
        }
    }

    public float Energy { 
        get {
            return _energy;
        } 
        set {
            _energy = value;
        } 
    }

    public float EnergyChargeRate {
        get {
            return _energyChargeRate;
        }
    }

    public float Speed {
        get {
            return _speed;
        }
    }

    public int TotalModCapacity {
        get {
            return _totalModCapacity;
        }
    }

    public bool Invincible { 
        get {
            return _invincible;
        } 
        set {
            _invincible = value;
        }
    }

    #endregion

    protected IMoveInput moveInput;
    protected Movement movement;
    protected ModBox weapons;
    protected ModBox specials;

    public void TakeDamage(float damage) {
        if (!_invincible) {
            _health -= damage;
            if (_health <= 0f) {
                Respawn();
            }
        }
    }

    /// <summary>
    /// Called by projectile or beam to show hitmarker on hit.
    /// </summary>
    public void HitMarker() {
        // TODO: Implement showing hitmarker
    }

    /// <summary>
    /// Returns the Vector3 that is being aimed at.
    /// </summary>
    public Vector3 AimTarget() {
        // TODO: Return the transform to shoot at
        throw new System.Exception("Not implemented!");
    }

    /// <summary>
    /// Process inputs and activate mods using the ModBox instances.
    /// </summary>
    protected void ProcessModActivation() {
        // TODO: Process mod activation before calculating movement
    }

    protected void LoadBaseShip() {
        moveInput = new GestureInput(rollRate, yawRate, pitchRate, transform);
        movement = new Movement(transform);
        // TODO: Load ModBox instances
    }

    protected void CalculateMovement() {
        ConvertInputs(moveInput);
        ApplyMovement(movement);
    }

    protected void Respawn() {
        // TODO: Implement networked respawning.
    }

    /// <summary>
    /// Processes an IMoveInput instance and updates the ship's Movement instance.
    /// </summary>
    void ConvertInputs(IMoveInput moveInput) {
        if (moveInput.ReadInputs) {
            moveInput.ProcessRawInput(transform);
            movement.ComputeNewTransform(transform, moveInput);
        }
    }

    /// <summary>
    /// Applies the Movement instance by updating the ship's Transform component.
    /// </summary>
    void ApplyMovement(Movement movement) {
        if (movement != null) {
            transform.position = movement.GetNewPosition();
            transform.rotation = movement.GetNewRotation();
        }
    }

}
