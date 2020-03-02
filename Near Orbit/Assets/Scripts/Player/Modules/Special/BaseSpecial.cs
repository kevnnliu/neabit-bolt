using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpecial : MonoBehaviour, IShipModule {
    private float duration;
    private float cooldown;
    private bool whilePressed;
    private float timer;

    private bool effectActive;

    protected BaseShip owner;

    public ModuleType Type => ModuleType.Special;

    public bool IsActive => timer > 0;

    public abstract void Init(BaseShip owner);

    protected void Init(BaseShip owner, float duration, float cooldown, bool whilePressed) {
        this.owner = owner;
        this.duration = duration;
        this.cooldown = cooldown;
        this.whilePressed = whilePressed;
    }

    public void Activate() {
        if (timer == 0 && !effectActive) {
            timer = duration;
            effectActive = true;
        }
    }

    public void Deactivate() {
        if (whilePressed && effectActive) {
            // TODO: Maybe replace with this?
            // timer = cooldown - timer;
            timer = cooldown;
            effectActive = false;
        }
    }

    public void Update() {
        timer = Mathf.Max(timer - Time.deltaTime, 0);
        if (timer == 0 && effectActive) {
            timer = cooldown;
            effectActive = false;
        }
        if (effectActive) {
            ApplyEffect();
        }
    }

    protected abstract void ApplyEffect();
}
