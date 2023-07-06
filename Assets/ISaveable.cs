using System.Collections;
using System.Collections.Generic;

public interface ISaveable
{
  public IDictionary<string, string> GetSaveData();
  public void SetSaveData(IDictionary<string, string> saveData);
}
