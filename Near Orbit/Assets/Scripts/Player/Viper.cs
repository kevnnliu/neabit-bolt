using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viper : BaseShip, IShip {

    #region Property Fields

    private const float _speed = 10f;
    private const int _totalModCapacity = 3;
    private const float _baseHealth = 100f;
    private const float _baseEnergy = 100f;
    private const float _energyChargeRate = 2.5f;

    private float _health;
    private float _energy;
    private bool _invincible;

    #endregion

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

    void Start() {
        LoadBaseShip();
    }

    void Update() {
        // TODO: Process mod activation before calculating movement.
        CalculateMovement();
    }

    public void TakeDamage(float damage) {
        if (!_invincible) {
            _health -= damage;
            if (_health <= 0f) {
                Respawn();
            }
        }
    }

    private void Respawn() {
        // TODO: Implement networked respawning.
    }

}
