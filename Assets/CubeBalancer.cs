using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBalancer : MonoBehaviour
{
    public float P = 1f;
    public float I = 0f;
    public float D = 0f;

    public float ScalingStrength = 0.5f;

    public Quaternion targetRot;

    private Quaternion targetRotation;

    private Quaternion previousError;
    private Quaternion integral;

    private Rigidbody rb;

    private void Start()
    {
        Debug.Log("AWJC CubeBalancer Loaded");
        Debug.Log(string.Format("Params: P {0} I {1} D {2}", P, I, D));
        rb = GetComponent<Rigidbody>();
        targetRotation = rb.rotation;

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
        Debug.Log(string.Format("{0}: {1}    (Q {2}, Normalized {3})", id, q.eulerAngles, q, q.normalized));
    }

    private void LogV(string id, Vector3 v) {
        Debug.Log(string.Format("{0}: {1}", id, v));
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
        float angle;
        Vector3 axis;
        error.ToAngleAxis(out angle, out axis);
        float mag = angle / 180.0f;
        Vector3 pForce = axis * mag * P;
        LogV("pForce", pForce);

        // Calculate the integral term
        // TODO(awjc): I term
        // Quaternion scaledError = Quaternion.Euler(error.eulerAngles * Time.fixedDeltaTime);
        // integral = (integral * scaledError).normalized;
        // Quaternion integralTerm = Quaternion.LerpUnclamped(Quaternion.identity, integral, I);
        Vector3 iForce = new Vector3(0.0f, 0.0f, 0.0f);

        // Get D force component by just inverting angular velocity and scaling
        Vector3 dForce = -rb.angularVelocity * D;
        LogV("Angular Velocity", rb.angularVelocity);
        LogV("dForce", dForce);

        // Calculate the control output by just summing each of the components
        Vector3 outputForce = pForce + iForce + dForce;
        LogV("outputForce", outputForce);

        // Multiply by a global scaling strength
        Vector3 scaled = outputForce * ScalingStrength;
        LogV("scaled", scaled);

        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

        // Apply the angular velocity as force to the object's rigidbody
        rb.AddTorque(scaled, ForceMode.Force);

        // Store the current error for the next frame
        previousError = error;
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
