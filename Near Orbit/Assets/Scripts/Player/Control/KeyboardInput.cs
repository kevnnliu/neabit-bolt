using UnityEngine;

public class KeyboardInput : IMoveInput
{

    private float mouseMoveSpeed = 0.005f;
    private Vector3 pitchYaw;
    private Vector3 lastPosition;
    private bool read;

    private Transform shipTransform;
    private Transform camera, reticlePoint;
    private float baseScale;
    private Vector3 aimPoint;

    public KeyboardInput(Transform shipT)
    {
        shipTransform = shipT;
        //camera = PlayerCamera.instance.GetTrackingSpace().Find("CenterEyeAnchor");
        reticlePoint = shipT.Find("MainReticle");
        baseScale = reticlePoint.localScale.x;
        lastPosition = new Vector3(Screen.width / 2, Screen.height / 2);
        UpdateInput();
    }

    public bool ReadInputs => true;

    public void UpdateInput()
    {
        read = false;
        //pitchYaw += mouseMoveSpeed * (Input.mousePosition - lastPosition);
        //lastPosition = Input.mousePosition;
        pitchYaw = mouseMoveSpeed * (Input.mousePosition - lastPosition);
        pitchYaw.x = Mathf.Clamp(pitchYaw.x, -1, 1);
        pitchYaw.y = Mathf.Clamp(pitchYaw.y, -1, 1);

        reticlePoint.position = GetReticlePoint();
        float newScale = Vector3.Distance(camera.position, reticlePoint.position) / ReticleAimConstants.MaxReticleDist;
        reticlePoint.localScale = Vector3.one * baseScale * newScale;
        reticlePoint.rotation = Quaternion.LookRotation(camera.position - reticlePoint.position, reticlePoint.up);
    }

    public Vector3 GetReticlePoint()
    {
        Vector3 aimVector = Quaternion.Euler(ReticleAimConstants.MaxFiringAngle * -pitchYaw.y,
            ReticleAimConstants.MaxFiringAngle * pitchYaw.x, 0) * Vector3.forward;

        aimVector = shipTransform.TransformDirection(aimVector);

        float angle = Vector3.Angle(aimVector, shipTransform.forward);
        if (angle > ReticleAimConstants.MaxFiringAngle)
        {
            Quaternion arc = Quaternion.FromToRotation(shipTransform.forward, aimVector);
            arc = Quaternion.Slerp(Quaternion.identity, arc, ReticleAimConstants.MaxFiringAngle / angle);
            aimVector = arc * shipTransform.forward;
        }

        bool hitSomething = Physics.Raycast(camera.position, aimVector, out RaycastHit hit, ReticleAimConstants.MaxReticleDist);

        if (hitSomething)
        {
            if (Vector3.Distance(camera.position, hit.point) < ReticleAimConstants.MinPointDist)
            {
                aimVector *= ReticleAimConstants.MinPointDist;
            }
            else
            {
                return hit.point;
            }
        }
        else
        {
            aimVector *= ReticleAimConstants.MaxReticleDist;
        }

        return camera.position + aimVector;
    }

    public void ComputeAimPoint(Vector3 reticlePosition)
    {
        Vector3 aimVector = (reticlePosition - camera.position).normalized;

        bool hitSomething = Physics.Raycast(camera.position, aimVector, out RaycastHit hit, ReticleAimConstants.MaxPointDist);

        if (hitSomething)
        {
            if (Vector3.Distance(camera.position, hit.point) < ReticleAimConstants.MinPointDist)
            {
                aimVector *= ReticleAimConstants.MinPointDist;
            }
            else
            {
                aimPoint = hit.point;
                return;
            }
        }
        else
        {
            aimVector *= ReticleAimConstants.MaxPointDist;
        }

        aimPoint = camera.position + aimVector;
    }

    public Vector3 GetAimPoint()
    {
        return aimPoint;
    }

    public void SetAimPoint(Vector3 aimPosition)
    {
        aimPoint = aimPosition;
    }

    public Vector3 GetRotationInput()
    {
        return new Vector3(GetPitchInput(), GetYawInput(), GetRollInput());
    }

    public float GetThrustInput() => Input.GetButton("Thrust") ? 1 : 0;

    public bool WeaponActivated() => Input.GetMouseButton(0);

    public bool WeaponNextPressed() => Input.GetKeyDown(KeyCode.E) && !read;

    public bool WeaponPrevPressed() => Input.GetKeyDown(KeyCode.Q) && !read;

    public int SpecialActivated(int index)
    {
        switch (index)
        {
            case 0:
                return Input.GetMouseButtonUp(1) ? -1
                    : Input.GetMouseButtonDown(1) ? 1
                    : 0;
            case 1:
                return Input.GetKeyDown(KeyCode.LeftShift) ? -1
                    : Input.GetKeyUp(KeyCode.LeftShift) ? 1
                    : 0;
            case 2:
                return Input.GetKeyDown(KeyCode.E) ? -1
                    : Input.GetKeyUp(KeyCode.E) ? 1
                    : 0;
            default:
                return 0;
        }
    }

    public void MarkAsRead() => read = true;

    public void ProcessRawInput(Transform shipT) { }

    private float GetRollInput()
    {
        return -Input.GetAxis("Horizontal");
    }

    private float GetYawInput()
    {
        return pitchYaw.x;
    }

    private float GetPitchInput()
    {
        return -pitchYaw.y;
    }

}
