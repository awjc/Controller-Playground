using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public interface ISaveable
{
  public string UniqueNameId();
  public IDictionary<string, object> ToJsonSaveData();
  public void FromJsonSaveData(IDictionary<string, object> jsonSaveData);
}
