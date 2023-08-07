using System.Collections.Generic;
using UnityEngine;


public class SaveWorldObj : MonoBehaviour, ISaveable
{
  public bool ThrowIfNotFound = false;

  public bool InclTransform = true;
  public bool InclVelocity = true;
  public bool InclAngularVelocity = true;

  public IDictionary<string, object> ToSaveData()
  {
    var saver = ReflectionUtil.MakeSaver(this);
    if (InclTransform)
    {
      saver.SaveTransform("transform");
    }
    var rb = GetComponent<Rigidbody>();
    if (InclVelocity && rb != null)
    {
      saver.SaveVector3("velocity", rb.velocity);
    }
    if (InclAngularVelocity && rb != null)
    {
      saver.SaveVector3("angularVelocity", rb.angularVelocity);
    }
    return saver.Data;
  }

  public void FromSaveData(IDictionary<string, object> saveData)
  {
    var loader = ReflectionUtil.MakeLoader(this, saveData, ThrowIfNotFound);
    if (InclTransform)
    {
      loader.LoadTransformInto("transform", this.transform);
    }
    var rb = GetComponent<Rigidbody>();
    if (InclVelocity && rb != null)
    {
      loader.LoadVector3("velocity", v => rb.velocity = v);
    }
    if (InclAngularVelocity && rb != null)
    {
      loader.LoadVector3("angularVelocity", av => rb.angularVelocity = av);
    }
  }
}
