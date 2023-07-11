using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class InterfacesUtil
{
  public static List<T> GetInterfaces<T>(GameObject objectToSearch) where T : class
  {
    MonoBehaviour[] mbComps = objectToSearch.GetComponents<MonoBehaviour>();
    var resultList = new List<T>();
    foreach (MonoBehaviour mb in mbComps)
    {
      if (mb is T)
      {
        resultList.Add((T)((System.Object)mb));
      }
    }
    return resultList;
  }

  public static List<T> GetAllInterfaces<T>(IEnumerable<GameObject> objectsToSearch) where T : class
  {
    return objectsToSearch.SelectMany(obj => GetInterfaces<T>(obj)).ToList();
  }
}
