using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;


public interface ISaveable
{
  public string GameObjectName()
  {
    // TODO(awjc) - use GetInstanceID().ToString() for something more globally unique ?
    if (!(this is MonoBehaviour))
    {
      throw new System.Exception("ISaveable isn't Monobehavior, can't populate GameObjectName() automatically");
    }
    return (this as MonoBehaviour).name;
  }

  public string ComponentName()
  {
    return GetType().Name;
  }

  public IDictionary<string, object> ToSaveData();
  public void FromSaveData(IDictionary<string, object> saveData);
}
