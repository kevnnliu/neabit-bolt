using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to a controller and handles point aiming.
/// </summary>
public class PointAim : MonoBehaviour {

    [SerializeField]
    private Transform eyeTrack;
    [SerializeField]
    private Transform reticlePoint;

    private const float intersectRadius = 4f;
    private const float minPointDist = 3f;
    private const float maxPointDist = 20f;
    private const float maxFiringAngle = 18f;

    private float baseScale;
    private BaseShip ship;

    void Start() {
        baseScale = reticlePoint.localScale.x;
        ship = transform.root.GetComponent<BaseShip>();
    }

    void Update() {
        reticlePoint.position = GetReticlePoint();
        float newScale = Vector3.Distance(transform.position, reticlePoint.position) / maxPointDist;
        reticlePoint.localScale = Vector3.one * baseScale * newScale;
        reticlePoint.rotation = Quaternion.LookRotation(eyeTrack.position - reticlePoint.position, reticlePoint.transform.up);
    }

    /// <summary>
    /// Returns the Vector3 position of the aim point.
    /// </summary>
    public Vector3 GetReticlePoint() {
        Vector3 vectorA = eyeTrack.position - transform.position;
        Vector3 vectorP = transform.forward * int.MaxValue;
        float degXYW = Vector3.Angle(vectorA, vectorP);

        float lenXW = Mathf.Sin(degXYW * Mathf.Deg2Rad) * vectorA.magnitude;
        float degZXW = Mathf.Acos(lenXW / intersectRadius) * Mathf.Rad2Deg;
        float degZXY = degZXW + (90f - degXYW);

        float lenYZ = LawOfCosines(vectorA.magnitude, intersectRadius, degZXY * Mathf.Deg2Rad);
        Vector3 intersectPosition = transform.position + (transform.forward * lenYZ);

        Vector3 aimVector = (intersectPosition - eyeTrack.position).normalized;

        float angle = Vector3.Angle(aimVector, ship.transform.forward);
        if (angle > maxFiringAngle) {
            Quaternion arc = Quaternion.FromToRotation(ship.transform.forward, aimVector);
            arc = Quaternion.Slerp(Quaternion.identity, arc, maxFiringAngle / angle);
            aimVector = arc * ship.transform.forward;
        }

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(eyeTrack.position, aimVector, out hit, maxPointDist);

        if (hitSomething) {
            if (Vector3.Distance(eyeTrack.position, hit.point) < minPointDist) {
                aimVector *= minPointDist;
            }
            else {
                return hit.point;
            }
        }
        else {
            aimVector *= maxPointDist;
        }

        return eyeTrack.position + aimVector;
    }

    /// <summary>
    /// Returns c in the law of cosines equation.
    /// </summary>
    private float LawOfCosines(float lenA, float lenB, float radC) {
        return Mathf.Sqrt(Mathf.Pow(lenA, 2) + Mathf.Pow(lenB, 2) - (2 * lenA * lenB * Mathf.Cos(radC)));
    }

}
