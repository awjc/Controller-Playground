using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/*

Ideas:
- Force vector visualization
- Multi-level balancing controller
-

*/


public class CubePIDBalancer : MonoBehaviour, ISaveable
{
  public float P = 11.0f;
  public float I = 0.0002f;
  public float D = 0.2f;

  public float ScalingStrength = 20f;

  public Quaternion targetRotation;

  private Vector3 integralTorques;

  private new Rigidbody rigidbody { get { return GetComponent<Rigidbody>(); } }

  public float errorAngle;
  public Vector3 errorAxis;

  public IDictionary<string, object> ToSaveData()
  {
    var saver = ReflectionUtil.MakeSaver(this);
    saver.Save("P");
    saver.Save("I");
    saver.Save("D");
    saver.Save("ScalingStrength");
    saver.SaveQuaternion("targetRotation");
    return saver.Data;
  }

  public void FromSaveData(IDictionary<string, object> saveData)
  {
    var loader = ReflectionUtil.MakeLoader(this, saveData);
    loader.LoadFloat("P");
    loader.LoadFloat("I");
    loader.LoadFloat("D");
    loader.LoadFloat("ScalingStrength");
    loader.LoadQuaternion("targetRotation");
  }

  private void Start()
  {
    Debug.Log("AWJC CubeBalancer Loaded");
    Debug.Log(string.Format("Params: P {0} I {1} D {2}", P, I, D));
    targetRotation = rigidbody.rotation;
  }

  private void LogQ(string id, Quaternion q)
  {
    // Debug.Log(string.Format("{0}: {1}    (Q {2}, Normalized {3})", id, q.eulerAngles, q, q.normalized));
  }

  private void LogV(string id, Vector3 v)
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

  private AngleAxis ToAngleAxis(Quaternion q)
  {
    q.ToAngleAxis(out float iAngle, out Vector3 iAxis);
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
    AngleAxis errorAA = ToAngleAxis(error);
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
    Vector3 dForce = -rigidbody.angularVelocity * D;
    LogV("Angular Velocity", rigidbody.angularVelocity);
    LogV("dForce", dForce);

    // Calculate the control output by just summing each of the components
    Vector3 outputForce = pForce + iForce + dForce;
    LogV("outputForce", outputForce);

    // Multiply by a global scaling strength
    Vector3 scaled = outputForce * ScalingStrength;
    // Haven't figured out how to make it framerate-independent yet..
    // * Time.fixedDeltaTime;
    LogV("scaled", scaled);

    if (System.Double.IsNaN(scaled.x) ||
    System.Double.IsNaN(scaled.y) ||
    System.Double.IsNaN(scaled.z))
    {
      return;
    }

    // Apply the angular velocity as force to the object's rigidbody
    rigidbody.AddTorque(scaled, ForceMode.Force);
  }

  private void FixedUpdate()
  {
    if (rigidbody == null)
    {
      Debug.Log("Null Rigidbody, skipping this update");
      return;
    }

    ApplyControllerForce();
  }

  private Vector3 CenterAngles(Vector3 input)
  {
    return new Vector3(Center(input.x), Center(input.y), Center(input.z));
  }

  private float Center(float angle)
  {
    return angle > 180 ? angle - 360 : angle;
  }

  public void SetTargetRotation(Quaternion target)
  {
    targetRotation = target;
  }
}
