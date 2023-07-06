using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;

/*

Ideas:
- Force vector visualization
- Multi-level balancing controller
-

*/


public class CubeBalancer : MonoBehaviour, ISaveable
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

  public float errorAngle;
  public Vector3 errorAxis;

  public string ToJsonSaveData()
  {
    // IDictionary<string, string> ret = new Dictionary<string, string>();

    // ret.Add('a', 'b');
    // ret.Add('c', 'd');

    // return ret;

    // string ret = JsonSerializer.Serialize(this);
    // return ret;
    return "awjc";
  }

  public void FromJsonSaveData(string jsonSaveData)
  {

  }

  private void Start()
  {
    Debug.Log("AWJC CubeBalancer Loaded");
    Debug.Log(string.Format("Params: P {0} I {1} D {2}", P, I, D));
    rb = GetComponent<Rigidbody>();
    targetRotation = rb.rotation;
    integral = Vector3.zero;
    integralAxis = Vector3.up;
  }

  private void TurnCube()
  {
    // targetRotation = Quaternion.Euler(0, 45, 0);
    targetRotation = targetRot;
    // Debug.Log("TURNING");
    // Vector3 torque = new Vector3(1.0f, 0.0f, 0.0f) * 10;
    // rb?.AddTorque(torque, ForceMode.Force);
  }

  private void LogQ(string id, Quaternion q)
  {
    // Debug.Log(string.Format("{0}: {1}    (Q {2}, Normalized {3})", id, q.eulerAngles, q, q.normalized));
  }

  private void LogV(string id, Vector3 v)
  {
    // Debug.Log(string.Format("{0}: {1}", id, v));
  }

  private void LogV2(string id, Vector3 v)
  {
    // Debug.Log(string.Format("{0}: {1}", id, v));
  }

  private class AngleAxis
  {
    public readonly float angle;
    public readonly Vector3 axis;

    public AngleAxis(float angle, Vector3 axis)
    {
      this.angle = angle;
      this.axis = axis;
    }
  }

  private AngleAxis toAngleAxis(Quaternion q)
  {
    float iAngle;
    Vector3 iAxis;
    q.ToAngleAxis(out iAngle, out iAxis);
    // Keep rotations below 180 degrees by performing the corresponding
    // rotation on the  opposite axis
    if (iAngle >= 180.0f)
    {
      iAngle = 360.0f - iAngle;
      iAxis = -iAxis;
    }
    return new AngleAxis(iAngle, iAxis);
  }

  public int frameCount = 0;

  private void ApplyControllerForce()
  {
    frameCount += 1;

    // Calculate the error between the current rotation and the target rotation
    Quaternion currentRotation = transform.rotation;
    Quaternion error = (targetRotation * Quaternion.Inverse(currentRotation)).normalized;
    LogQ("Error", error);

    // Calculate the proportional term
    AngleAxis errorAA = toAngleAxis(error);
    errorAxis = errorAA.axis;
    errorAngle = errorAA.angle;

    float tol = 1e-3f;
    if (errorAA.angle < tol || Mathf.Abs(errorAA.angle - 360.0f) < tol)
    {
      return;
    }
    float mag = errorAA.angle / 180.0f;
    Vector3 pForce = errorAA.axis * mag * P;
    LogV("pForce", pForce);

    // Calculate the integral term
    integralTorques += errorAA.angle * errorAA.axis;
    Vector3 iForce = integralTorques * I;
    LogV("iForce", iForce);

    // Get D force component by just inverting angular velocity and scaling
    Vector3 dForce = -rb.angularVelocity * D;
    LogV("Angular Velocity", rb.angularVelocity);
    LogV("dForce", dForce);

    // Calculate the control output by just summing each of the components
    Vector3 outputForce = pForce + iForce + dForce;
    LogV("outputForce", outputForce);

    // Multiply by a global scaling strength
    Vector3 scaled = outputForce * ScalingStrength;
    LogV2("scaled", scaled);

    if (System.Double.IsNaN(scaled.x) ||
    System.Double.IsNaN(scaled.y) ||
    System.Double.IsNaN(scaled.z))
    {
      return;
    }

    // Apply the angular velocity as force to the object's rigidbody
    rb.AddTorque(scaled, ForceMode.Force);
}

  private void FixedUpdate()
  {
    if (rb == null)
    {
      Debug.Log("Null Rigidbody, skipping this update");
      return;
    }

    ApplyControllerForce();
  }

  private Vector3 centerAngles(Vector3 input)
  {
    return new Vector3(center(input.x), center(input.y), center(input.z));
  }

  private float center(float angle)
  {
    return angle > 180 ? angle - 360 : angle;
  }

  public void SetTargetRotation(Quaternion target)
  {
    targetRotation = target;
  }
}
