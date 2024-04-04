using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

/*

Ideas:
- Force vector visualization
- Multi-level balancing controller
-

*/


public class CubePIDBalancer : MonoBehaviour, ISaveable
{
  public int DebugLoggingFrameCount = 0;

  public float P = 6.0f;
  public float I = 10.0f;
  public float D = 2.0f;

  public float ScalingStrength = 0.1f;

  public Quaternion targetRotation;

  private Vector3 integralTorques;

#pragma warning disable 0109 // For "new is not required" warning
  private new Rigidbody rigidbody { get { return GetComponent<Rigidbody>(); } }
#pragma warning restore 0109

  public float errorAngle;
  public Vector3 errorAxis;

  public int frameCount = 0;

  private Vector3 lastError = Vector3.zero;

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

    // targetRotation = rigidbody.rotation;

    // targetRotation = RandomRotation();
    // targetRotation = Quaternion.Euler(10f, 148f, 77f);

    targetRotation = Quaternion.Euler(30f, 45f, 55f);
  }

  public static Quaternion RandomRotation()
  {
    return Quaternion.Euler(
      Random.Range(-180, 180),
      Random.Range(-180, 180),
      Random.Range(-180, 180));
  }

  private void DebugLog(string str)
  {
    if (DebugLoggingFrameCount > 0 && frameCount % DebugLoggingFrameCount == 0)
    {
      Debug.Log(str);
    }
  }

  private void LogQ(string id, Quaternion q)
  {
    DebugLog(string.Format("{0}: {1}    (Q {2}, Normalized {3})", id, q.eulerAngles, q, q.normalized));
    var aa = ToAngleAxis(q);
    DebugLog(string.Format("\tAngle: {0}, Axis: {1}", aa.angle, aa.axis));
  }

  private void LogV(string id, Vector3 v)
  {
    DebugLog(string.Format("{0}: {1}", id, v));
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

  private void ApplyControllerForce()
  {
    frameCount += 1;
    DebugLog($"~~~~~~~~~~~~~~~~~~~~   FRAME {frameCount}   ~~~~~~~~~~~~~~~~~~~~");

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
    Vector3 proportionalError = errorAA.axis.normalized * errorAA.angle;
    Vector3 pForce = errorAA.angle * errorAA.axis;
    LogV("pForce", pForce);

    // Calculate the integral term
    integralTorques += errorAA.angle * Time.fixedDeltaTime * errorAA.axis;
    Vector3 iForce = integralTorques;
    LogV("iForce", iForce);

    // Get D force component by just inverting angular velocity and scaling
    Vector3 dForce = (proportionalError - lastError) / Time.fixedDeltaTime;
    // LogV("Angular Velocity", rigidbody.angularVelocity);
    LogV("dForce", dForce);

    // Calculate the control output by just summing each of the components
    Vector3 outputForce = P * pForce + I * iForce + D * dForce;
    LogV("outputForce", outputForce);

    // Multiply by a global scaling strength
    Vector3 scaled = outputForce * ScalingStrength;
    LogV("scaled", scaled);

    if (System.Double.IsNaN(scaled.x) ||
    System.Double.IsNaN(scaled.y) ||
    System.Double.IsNaN(scaled.z))
    {
      return;
    }

    // Apply the angular velocity as force to the object's rigidbody
    rigidbody.AddTorque(scaled, ForceMode.Force);

    lastError = proportionalError;
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
