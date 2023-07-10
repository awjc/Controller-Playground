using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SaverLoader : MonoBehaviour
{

  public InputActionAsset actions;

  private InputActionMap gameplayActionMap;

  private List<InputAction> allActions;

  void Start()
  {
    gameplayActionMap = actions.FindActionMap("SaveLoad", true);
    if (gameplayActionMap == null)
    {
      Debug.Log("Couldn't find SaveLoad action map. Aborting.");
      return;
    }

    var saveAction = gameplayActionMap.FindAction("Save");
    saveAction.performed += context => DoSave();

    var loadAction = gameplayActionMap.FindAction("Load");
    loadAction.performed += context => DoLoad();

    allActions = new List<InputAction> {
      saveAction, loadAction
    };

    Debug.Log("SaverLoader controls registered successfully.");
  }

  public void DoSave()
  {
    string destination = Application.persistentDataPath + "/save.txt";
    Debug.Log(string.Format("Saving to {0}", destination));

    List<GameObject> allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>().ToList();
    var allSaveable = Interfaces.GetAllInterfaces<ISaveable>(allObjects);

    // File structure is { gameobject: { component: <componentDict> } }
    var objDict = new Dictionary<string, IDictionary<string, IDictionary<string, object>>>();

    foreach (ISaveable saveObj in allSaveable)
    {
      var goName = saveObj.GameObjectName();
      var compName = saveObj.ComponentName();
      var data = saveObj.ToSaveData();
      if (!objDict.ContainsKey(goName))
      {
        objDict[goName] = new Dictionary<string, IDictionary<string, object>>();
      }
      objDict[goName][compName] = data;
    }

    var fileData = JsonConvert.SerializeObject(objDict);
    System.IO.File.WriteAllText(destination, fileData);
  }

  public void DoLoad()
  {
    string destination = Application.persistentDataPath + "/save.txt";
    Debug.Log(string.Format("Loading from {0}", destination));
    var data = System.IO.File.ReadAllText(destination);
    // File structure is { gameobject: { component: <componentDict> } }
    var objDict = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, IDictionary<string, object>>>>(data);

    foreach (var kv in objDict)
    {
      var go = GameObject.Find(kv.Key);
      if (go != null)
      {
        var allSaveable = Interfaces.GetInterfaces<ISaveable>(go);
        foreach (var saveable in allSaveable)
        {
          var compName = saveable.ComponentName();
          if (kv.Value.ContainsKey(compName))
          {
            saveable.FromSaveData(kv.Value[compName]);
          }
        }
      }
    }
  }

  private void OnEnable()
  {
    actions?.Enable();
    if (allActions != null)
    {
      foreach (var action in allActions)
      {
        action?.Enable();
      }
    }
  }

  private void OnDisable()
  {
    actions?.Disable();
    if (allActions != null)
    {
      foreach (var action in allActions)
      {
        action?.Disable();
      }
    }
  }
}
