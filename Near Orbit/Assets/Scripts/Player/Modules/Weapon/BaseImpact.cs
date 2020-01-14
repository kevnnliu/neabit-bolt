using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseImpact : MonoBehaviour {

    [SerializeField]
    private float duration;

    void Start() {
        
    }

    void Update() {
        duration -= Time.deltaTime;
        if (duration <= 0) {
            Destroy(this.gameObject);
        }
    }

}
