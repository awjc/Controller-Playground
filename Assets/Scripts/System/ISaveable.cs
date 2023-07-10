using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public interface ISaveable
{
  public string UniqueNameId();
  public IDictionary<string, object> ToSaveData();
  public void FromSaveData(IDictionary<string, object> jsonSaveData);
}
