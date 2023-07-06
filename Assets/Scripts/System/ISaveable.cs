using System.Collections;
using System.Collections.Generic;

public interface ISaveable
{
  public string ToJsonSaveData();
  public void FromJsonSaveData(string jsonSaveData);
}