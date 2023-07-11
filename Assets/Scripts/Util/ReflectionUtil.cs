using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class ReflectionUtil
{
  private static FieldInfo GetFieldByName(object obj, string fieldName)
  {
    return obj.GetType().GetField(fieldName,
      BindingFlags.NonPublic
        | BindingFlags.Public
        | BindingFlags.Instance
        | BindingFlags.FlattenHierarchy);
  }

  private static PropertyInfo GetPropertyByName(object obj, string fieldName)
  {
    return obj.GetType().GetProperty(fieldName,
      BindingFlags.NonPublic
        | BindingFlags.Public
        | BindingFlags.Instance
        | BindingFlags.FlattenHierarchy);
  }

  public static void Set(object obj, string fieldName, object fieldVal, bool throwIfNotFound = true)
  {
    var field = GetFieldByName(obj, fieldName);
    if (field != null)
    {
      field.SetValue(obj, fieldVal);
    }
    else
    {
      var prop = GetPropertyByName(obj, fieldName);
      if (prop != null)
      {
        prop.SetValue(obj, fieldVal);
      }
      else
      {
        if (throwIfNotFound)
        {
          throw new MissingFieldException(string.Format("No field/property found named `{0}` on object {1}", fieldName, obj));
        }
      }
    }
  }

  public static void Set(object obj, string fieldName, object fieldVal)
  {
    Set(obj, fieldName, fieldVal, true);
  }

  public static void SetIfPresent(object obj, string fieldName, object fieldVal)
  {
    Set(obj, fieldName, fieldVal, false);
  }

  public static object Get(object obj, string fieldName, bool throwIfNotFound = true)
  {
    var field = GetFieldByName(obj, fieldName);
    if (field != null)
    {
      return field.GetValue(obj);
    }
    else
    {
      var prop = GetPropertyByName(obj, fieldName);
      if (prop != null)
      {
        return prop.GetValue(obj);
      }
      else
      {
        if (throwIfNotFound)
        {
          throw new MissingFieldException(string.Format("No field/property found named `{0}` on object {1}", fieldName, obj));
        }
        else
        {
          return null;
        }
      }
    }
  }

  // ---------------------------------------------------------------------------

  public class Saver
  {
    private object obj { get; set; }
    private IDictionary<string, object> saveData { get; set; }
    public IDictionary<string, object> Data { get { return saveData; } }
    private bool throwIfNotFound { get; set; }
    public Saver(object obj, bool throwIfNotFound)
    {
      this.obj = obj;
      this.saveData = new Dictionary<string, object>();
      this.throwIfNotFound = throwIfNotFound;
    }

    private object GetFieldValue(string fieldName)
    {
      return Get(obj, fieldName, throwIfNotFound);
    }

    private T GetFieldValueAsObj<T>(string fieldName)
    {
      var val = GetFieldValue(fieldName);
      var asObj = (T)val;
      if (asObj != null)
      {
        return asObj;
      }
      else
      {
        throw new System.InvalidCastException(string.Format("Can't cast `{0}` as `{1}`", val, typeof(T)));
      }
    }

    public void Save(string fieldName)
    {
      var val = GetFieldValue(fieldName);
      saveData.Add(fieldName, val);
    }

    public void Save<T>(string fieldName, Func<T, object> converter)
    {
      T val = GetFieldValueAsObj<T>(fieldName);
      saveData.Add(fieldName, converter(val));
    }

    public void SaveQuaternion(string fieldName)
    {
      Save<Quaternion>(fieldName, val => SerializedQuaternion.Box(val));
    }

    public void SaveTransform(string fieldName)
    {
      Save<Transform>(fieldName, val => SerializedTransform.Box(val));
    }
  }

  public static Saver MakeSaver(object obj, bool throwIfNotFound = true)
  {
    return new Saver(obj, throwIfNotFound);
  }

  // ---------------------------------------------------------------------------

  public class Loader
  {
    private object obj { get; set; }
    private IDictionary<string, object> saveData { get; set; }
    private bool throwIfNotFound { get; set; }
    public Loader(object obj, IDictionary<string, object> saveData, bool throwIfNotFound)
    {
      this.obj = obj;
      this.saveData = saveData;
      this.throwIfNotFound = throwIfNotFound;
    }

    private object GetSaveData(string fieldName)
    {
      if (saveData == null || !saveData.ContainsKey(fieldName))
      {
        if (throwIfNotFound)
        {
          throw new System.MissingFieldException(string.Format("No save data found by field name `{0}`", fieldName));
        }
        else
        {
          return null;
        }
      }
      return saveData[fieldName];
    }

    private T GetSaveDataAsObj<T>(string fieldName) where T : class
    {
      var val = GetSaveData(fieldName);
      var asObj = ToObject<T>(val);
      if (asObj != null)
      {
        return asObj;
      }
      else
      {
        throw new System.InvalidCastException(string.Format("Can't cast `{0}` as `{1}`", val, typeof(T)));
      }
    }

    private void SetFromSaveData<T>(string fieldName, Func<object, T> converter)
    {
      var val = GetSaveData(fieldName);
      if (val != null)
      {
        Set(obj, fieldName, converter(val), throwIfNotFound);
      }
    }

    private void SetObjFromSaveData<T, S>(string fieldName, Func<T, S> converter) where T : class
    {
      var asObj = GetSaveDataAsObj<T>(fieldName);
      Set(obj, fieldName, converter(asObj), throwIfNotFound);
    }

    private void LoadIntoObjFromSaveData<T>(string fieldName, Action<T> loader) where T : class
    {
      var asObj = GetSaveDataAsObj<T>(fieldName);
      loader(asObj);
    }

    private T ToObject<T>(object obj) where T : class
    {
      return (obj as JObject)?.ToObject<T>();
    }

    public void LoadFloat(string fieldName)
    {
      SetFromSaveData<float>(fieldName, val => Convert.ToSingle(val));
    }

    public void LoadQuaternion(string fieldName)
    {
      SetObjFromSaveData<SerializedQuaternion, Quaternion>(fieldName, sq => sq.Unbox());
    }

    public void LoadTransformInto(string fieldName, Transform dest)
    {
      LoadIntoObjFromSaveData<SerializedTransform>(fieldName, st => st.UnboxInto(dest));
    }
  }

  public static Loader MakeLoader(object obj, IDictionary<string, object> saveData = null, bool throwIfNotFound = true)
  {
    return new Loader(obj, saveData, true);
  }
}
