using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BaseShip : MonoBehaviour {

    #region Serialized Fields
    
    [SerializeField]
    private int totalModCapacity;
    [SerializeField]
    private float baseHealth;
    [SerializeField]
    private ShipStats stats;
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

    void Awake() {
        SetupCamera();
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

    public Movement GetMovement() {
        return movement;
    }

    public IMoveInput GetMoveInput() {
        return input;
    }

    public void SetInvincibility(bool enabled) {
        invincible = enabled;
    }

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
        modules.AddModule(new LaserBlasterWeapon(this));
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
            transform.Find("OVRCameraRig").gameObject.SetActive(true);
            input = new GestureInput(transform);
        }
        else {
            XRSettings.enabled = false;
            transform.Find("Camera").gameObject.SetActive(true);
            input = new KeyboardInput(transform);
        }
    }

}
