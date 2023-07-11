using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTurnOn : MonoBehaviour, ISaveable
{
  public float startDelayTimeSec;

  private Rigidbody rb;

  public IDictionary<string, object> ToSaveData()
  {
    var saver = ReflectionUtil.MakeSaver(this);
    saver.Save("startDelayTimeSec");
    return saver.Data;
  }

  public void FromSaveData(IDictionary<string, object> saveData)
  {
    var loader = ReflectionUtil.MakeLoader(this, saveData);
    loader.LoadFloat("startDelayTimeSec");
  }

  // Start is called before the first frame update
  void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("TurnOnGravity", startDelayTimeSec);
    }

    private void TurnOnGravity() {
        if (rb == null) {
            Debug.Log("No RigidBody attached, cannot turn on gravity");
            return;
        }

        rb.useGravity = true;
    }
}
