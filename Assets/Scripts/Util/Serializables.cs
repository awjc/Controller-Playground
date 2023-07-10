using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class SerializedVector3
{
  [JsonProperty]
  private float[] vec3;

  [JsonConstructor]
  private SerializedVector3(float[] vector3)
  {
    this.vec3 = vector3;
  }

  public static SerializedVector3 Box(Vector3 inV)
  {
    return new SerializedVector3(new float[] { inV.x, inV.y, inV.z });
  }

  public Vector3 Unbox()
  {
    return new Vector3(vec3[0], vec3[1], vec3[2]);
  }
}

[Serializable]
public class SerializedQuaternion
{
  [JsonProperty]
  private float[] quat;

  [JsonConstructor]
  private SerializedQuaternion(float[] quaternion)
  {
    this.quat = quaternion;
  }

  public static SerializedQuaternion Box(Quaternion inQ)
  {
    return new SerializedQuaternion(new float[] { inQ.x, inQ.y, inQ.z, inQ.w });
  }

  public Quaternion Unbox()
  {
    return new Quaternion(quat[0], quat[1], quat[2], quat[3]);
  }
}

[Serializable]
public class SerializedTransform
{
  [JsonProperty]
  private SerializedVector3 position;
  [JsonProperty]
  private SerializedQuaternion rotation;
  [JsonProperty]
  private SerializedVector3 scale;

  [JsonConstructor]
  private SerializedTransform(SerializedVector3 position, SerializedQuaternion rotation, SerializedVector3 scale)
  {
    this.position = position;
    this.rotation = rotation;
    this.scale = scale;
  }

  public static SerializedTransform Box(Transform transform)
  {
    return new SerializedTransform(
      SerializedVector3.Box(transform.localPosition),
      SerializedQuaternion.Box(transform.localRotation),
      SerializedVector3.Box(transform.localScale));
  }

  public void UnboxTo(Transform transform)
  {
    transform.SetLocalPositionAndRotation(position.Unbox(), rotation.Unbox());
    transform.localScale = scale.Unbox();
  }
}
