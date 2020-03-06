using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to a controller and handles point aiming.
/// </summary>
public class PointAim {

    private Transform eyeTrack;
    private Transform rightController;
    private Transform reticlePoint;
    private float baseScale;
    private BaseShip ship;

    public PointAim(Transform shipT) {
        Transform trackSpace = shipT.Find("OVRCameraRig").Find("TrackingSpace");
        eyeTrack = trackSpace.Find("CenterEyeAnchor");
        rightController = trackSpace.Find("RightHandAnchor");
        reticlePoint = shipT.Find("MainReticle");
        baseScale = reticlePoint.localScale.x;
        ship = shipT.GetComponent<BaseShip>();
    }
    public void UpdateAim() {
        reticlePoint.position = GetReticlePoint();
        float newScale = Vector3.Distance(rightController.position, reticlePoint.position) / ReticleAimConstants.MaxReticleDist;
        reticlePoint.localScale = Vector3.one * baseScale * newScale;
        reticlePoint.rotation = Quaternion.LookRotation(eyeTrack.position - reticlePoint.position, reticlePoint.up);
    }

    /// <summary>
    /// Returns the Vector3 position of the aim point.
    /// NEEDS TO BE MOVED TO SERVER-SIDE
    /// </summary>
    public Vector3 GetAimPoint() {
        Vector3 aimVector = (reticlePoint.position - eyeTrack.position).normalized;

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(eyeTrack.position, aimVector, out hit, ReticleAimConstants.MaxPointDist);

        return ClampRay(hitSomething, hit, aimVector, ReticleAimConstants.MaxPointDist);
    }

    /// <summary>
    /// Returns the Vector3 position of the reticle.
    /// </summary>
    public Vector3 GetReticlePoint() {
        Vector3 vectorA = eyeTrack.position - rightController.position;
        Vector3 vectorP = rightController.forward * int.MaxValue;
        float degXYW = Vector3.Angle(vectorA, vectorP);

        float lenXW = Mathf.Sin(degXYW * Mathf.Deg2Rad) * vectorA.magnitude;
        float degZXW = Mathf.Acos(lenXW / ReticleAimConstants.IntersectRadius) * Mathf.Rad2Deg;
        float degZXY = degZXW + (90f - degXYW);

        float lenYZ = LawOfCosines(vectorA.magnitude, ReticleAimConstants.IntersectRadius, degZXY * Mathf.Deg2Rad);
        Vector3 intersectPosition = rightController.position + (rightController.forward * lenYZ);

        Vector3 aimVector = (intersectPosition - eyeTrack.position).normalized;

        float angle = Vector3.Angle(aimVector, ship.transform.forward);
        if (angle > ReticleAimConstants.MaxFiringAngle) {
            aimVector = ClampAimAngle(angle, aimVector);
        }

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(eyeTrack.position, aimVector, out hit, ReticleAimConstants.MaxReticleDist);

        return ClampRay(hitSomething, hit, aimVector, ReticleAimConstants.MaxReticleDist);
    }

    /// <summary>
    /// Returns c in the law of cosines equation.
    /// </summary>
    private float LawOfCosines(float lenA, float lenB, float radC) {
        return Mathf.Sqrt(Mathf.Pow(lenA, 2) + Mathf.Pow(lenB, 2) - (2 * lenA * lenB * Mathf.Cos(radC)));
    }

    /// <summary>
    /// Clamps the raycast hit distance, returns the correct ray endpoint.
    /// </summary>
    private Vector3 ClampRay(bool hitSomething, RaycastHit hit, Vector3 aimVector, float maxDist) {
        if (hitSomething) {
            if (Vector3.Distance(eyeTrack.position, hit.point) < ReticleAimConstants.MinPointDist) {
                aimVector *= ReticleAimConstants.MinPointDist;
            } else {
                return hit.point;
            }
        } else {
            aimVector *= maxDist;
        }

        return eyeTrack.position + aimVector;
    }

    private Vector3 ClampAimAngle(float angle, Vector3 aimVector) {
        Quaternion arc = Quaternion.FromToRotation(ship.transform.forward, aimVector);
        arc = Quaternion.Slerp(Quaternion.identity, arc, ReticleAimConstants.MaxFiringAngle / angle);
        return arc * ship.transform.forward;
    }

}
