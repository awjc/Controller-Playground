using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class SerializedTransform
{
  public float[] _position = new float[3];
  public float[] _rotation = new float[4];
  public float[] _scale = new float[3];

  [JsonConstructor]
  public SerializedTransform(float[] position, float[] rotation, float[] scale)
  {
    this._position = position;
    this._rotation = rotation;
    this._scale = scale;
  }

  public SerializedTransform(Transform transform)
  {
    _position[0] = transform.localPosition.x;
    _position[1] = transform.localPosition.y;
    _position[2] = transform.localPosition.z;

    _rotation[0] = transform.localRotation.x;
    _rotation[1] = transform.localRotation.y;
    _rotation[2] = transform.localRotation.z;
    _rotation[3] = transform.localRotation.w;

    _scale[0] = transform.localScale.x;
    _scale[1] = transform.localScale.y;
    _scale[2] = transform.localScale.z;
  }

  public void CopyToTransform(Transform transform)
  {
    transform.SetLocalPositionAndRotation(
      new Vector3(_position[0], _position[1], _position[2]),
      new Quaternion(_rotation[0], _rotation[1], _rotation[2], _rotation[3]));
    transform.localScale = new Vector3(_scale[0], _scale[1], _scale[2]);
  }
}
