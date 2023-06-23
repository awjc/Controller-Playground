using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

Ideas:
    - Force vector visualization
    - Multi-level balancing controller
    -

*/


public class CubeBalancer : MonoBehaviour
{
    public float P = 1f;
    public float I = 0f;
    public float D = 0f;

    public float ScalingStrength = 0.5f;

    public Quaternion targetRot;

    public Quaternion targetRotation;

    private Quaternion previousError;

    private Vector3 integral;

    private float integralAngle;

    private Vector3 integralAxis;

    private Vector3 integralTorques;

    private Rigidbody rb;

    private void Start()
    {
        Debug.Log("AWJC CubeBalancer Loaded");
        Debug.Log(string.Format("Params: P {0} I {1} D {2}", P, I, D));
        rb = GetComponent<Rigidbody>();
        targetRotation = rb.rotation;
        integral = Vector3.zero;
        integralAxis = Vector3.up;

        // Invoke("TurnCube", 2.0   f);
    }

    private void TurnCube() {
        // targetRotation = Quaternion.Euler(0, 45, 0);
        targetRotation = targetRot;
        // Debug.Log("TURNING");
        // Vector3 torque = new Vector3(1.0f, 0.0f, 0.0f) * 10;
        // rb?.AddTorque(torque, ForceMode.Force);
    }

    private void LogQ(string id, Quaternion q) {
        // Debug.Log(string.Format("{0}: {1}    (Q {2}, Normalized {3})", id, q.eulerAngles, q, q.normalized));
    }

     private void LogV(string id, Vector3 v) {
        // Debug.Log(string.Format("{0}: {1}", id, v));
    }

    private void LogV2(string id, Vector3 v) {
        Debug.Log(string.Format("{0}: {1}", id, v));
    }

    private class AngleAxis {
        public readonly float angle;
        public readonly Vector3 axis;

        public AngleAxis(float angle, Vector3 axis) {
            this.angle = angle;
            this.axis = axis;
        }
    }

    private AngleAxis toAngleAxis(Quaternion q) {
        float iAngle;
        Vector3 iAxis;
        q.ToAngleAxis(out iAngle, out iAxis);
        return new AngleAxis(iAngle, iAxis);
    }

    private void FixedUpdate()
    {
        if (rb == null) {
            Debug.Log("Null Rigidbody, skipping this update");
            return;
        }

        // Calculate the error between the current rotation and the target rotation
        Quaternion currentRotation = transform.rotation;
        Quaternion error = (targetRotation * Quaternion.Inverse(currentRotation)).normalized;
        LogQ("Error", error);

        // Calculate the proportional term
        AngleAxis errorAA = toAngleAxis(error);
        float mag = errorAA.angle / 180.0f;
        Vector3 pForce = errorAA.axis * mag * P;
        LogV2("pForce", pForce);

        // Calculate the integral term
        // TODO(awjc): I term
        // Quaternion scaledError = Quaternion.Euler(error.eulerAngles * Time.fixedDeltaTime);
        // integral = (integral * scaledError).normalized;
        // Quaternion integralTerm = Quaternion.LerpUnclamped(Quaternion.identity, integral, I);


        AngleAxis prevErrAA = toAngleAxis(previousError);

        // Debug.Log(string.Format("$$$$$$$$$$$  {0}  vs  {1}     ({2}) ##############", prevErrAA.axis, errorAA.axis, Time.fixedDeltaTime));

        // TODO(awjc): Last N errors instead of cumulative?
        // TODO(awjc): Diminishing geometric decay, *= 0.9f each frame
        // integralAxis = errorAA
        // integralAngle += errorAA.angle * Time.fixedDeltaTime;

        integralTorques += errorAA.angle * errorAA.axis;

        // LogV2("integralTorques", integralTorques);
        // integralAngle = (integralAngle * 10 + errorAA.angle * Time.fixedDeltaTime) / 10;

        // float iProp = (integralAngle + errorAA.angle == 0) ? 0 :
        //         integralAngle / (integralAngle + errorAA.angle);

        // integralAxis = Vector3.Lerp(errorAA.axis, integralAxis, iProp);

        // Vector3 iForce = Vector3.zero;
        // Vector3 iForce = integralAxis * integralAngle * Time.fixedDeltaTime * I;
        Vector3 iForce = integralTorques * I;
        LogV2("iForce", iForce);

        // if (integralAngle > 1e-5f) {
        //     float ip = -(Mathf.Log(integralAngle) - 1) / 5;
        //     float factor = Mathf.Lerp(0.999f, 0.95f, ip);
        //     Debug.Log(string.Format("{0} , {1} , {2}", integralAngle, ip, factor));
        //     integralAngle *= factor;
        // }

        // Debug.Log(string.Format("{0} , {1} , {2}", integralAngle, integralAxis, iForce));

        // Get D force component by just inverting angular velocity and scaling
        Vector3 dForce = -rb.angularVelocity * D;
        LogV("Angular Velocity", rb.angularVelocity);
        LogV2("dForce", dForce);

        // Calculate the control output by just summing each of the components
        Vector3 outputForce = pForce + iForce + dForce;
        LogV("outputForce", outputForce);

        // Multiply by a global scaling strength
        Vector3 scaled = outputForce * ScalingStrength;
        LogV2("scaled", scaled);

        // Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

        // Apply the angular velocity as force to the object's rigidbody
        rb.AddTorque(scaled, ForceMode.Force);

        // Store the current error for the next frame
        // previousError = error;
    }

    private Vector3 centerAngles(Vector3 input) {
        return new Vector3(center(input.x), center(input.y), center(input.z));
    }

    private float center(float angle) {
        return angle > 180 ? angle - 360 : angle;
    }

    public void SetTargetRotation(Quaternion target)
    {
        targetRotation = target;
    }
}
