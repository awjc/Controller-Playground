using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class SerializedVector3
{
  [JsonProperty]
  private float[] _v3;

  [JsonConstructor]
  private SerializedVector3(float[] _vector3)
  {
    this._v3 = _vector3;
  }

  public static SerializedVector3 Box(Vector3 inV)
  {
    return new SerializedVector3(new float[] { inV.x, inV.y, inV.z });
  }

  public Vector3 Unbox()
  {
    return new Vector3(_v3[0], _v3[1], _v3[2]);
  }
}

[Serializable]
public class SerializedQuaternion
{
  [JsonProperty]
  private float[] _q;

  [JsonConstructor]
  private SerializedQuaternion(float[] quaternion)
  {
    this._q = quaternion;
  }

  public static SerializedQuaternion Box(Quaternion inQ)
  {
    return new SerializedQuaternion(new float[] { inQ.x, inQ.y, inQ.z, inQ.w });
  }

  public Quaternion Unbox()
  {
    return new Quaternion(_q[0], _q[1], _q[2], _q[3]);
  }
}

[Serializable]
public class SerializedTransform
{
  [JsonProperty]
  private SerializedVector3 _position;
  [JsonProperty]
  private SerializedQuaternion _rotation;
  [JsonProperty]
  private SerializedVector3 _scale;

  [JsonConstructor]
  private SerializedTransform(SerializedVector3 position, SerializedQuaternion rotation, SerializedVector3 scale)
  {
    this._position = position;
    this._rotation = rotation;
    this._scale = scale;
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
    transform.SetLocalPositionAndRotation(_position.Unbox(), _rotation.Unbox());
    transform.localScale = _scale.Unbox();
  }
}
