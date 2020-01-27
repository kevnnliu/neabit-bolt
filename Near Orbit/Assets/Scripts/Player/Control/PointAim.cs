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
    private Transform aimPoint;

    private const float intersectRadius = 4f;
    private const float minPointDist = 2f;
    private const float maxPointDist = 20f;

    private float baseScale;

    void Start() {
        baseScale = aimPoint.localScale.x;
    }

    void Update() {
        aimPoint.position = GetAimPoint();
        float newScale = Vector3.Distance(transform.position, aimPoint.position) / maxPointDist;
        aimPoint.localScale = Vector3.one * baseScale * newScale;
        aimPoint.rotation = Quaternion.LookRotation(eyeTrack.position - aimPoint.position, aimPoint.transform.up);
    }

    /// <summary>
    /// Returns the Vector3 position of the aim point.
    /// </summary>
    public Vector3 GetAimPoint() {
        Vector3 vectorA = eyeTrack.position - transform.position;
        Vector3 vectorP = transform.forward * int.MaxValue;
        float degXYW = Vector3.Angle(vectorA, vectorP);

        float lenXW = Mathf.Sin(degXYW * Mathf.Deg2Rad) * vectorA.magnitude;
        float degZXW = Mathf.Acos(lenXW / intersectRadius) * Mathf.Rad2Deg;
        float degZXY = degZXW + (90f - degXYW);

        float lenYZ = LawOfCosines(vectorA.magnitude, intersectRadius, degZXY * Mathf.Deg2Rad);
        Vector3 intersectPosition = transform.position + (transform.forward * lenYZ);

        Vector3 aimVector = (intersectPosition - eyeTrack.position).normalized;
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
