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
    Debug.Log("Saving...");

    List<GameObject> allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>().ToList();
    var allSaveable = Interfaces.GetAllInterfaces<ISaveable>(allObjects);

    var objDict = new Dictionary<string, IDictionary<string, object>>();

    foreach (ISaveable saveObj in allSaveable)
    {
      Debug.Log(string.Format("{0} : {1}", saveObj.UniqueNameId(), saveObj.ToJsonSaveData()));
      objDict[saveObj.UniqueNameId()] = saveObj.ToJsonSaveData();
    }

    Debug.Log(string.Join(", ", allObjects));
    Debug.Log(string.Join(", ", allSaveable));

    var fileData = JsonConvert.SerializeObject(objDict);
    SaveFile(fileData);
  }

  public void SaveFile(string data)
  {
    string destination = Application.persistentDataPath + "/save.txt";
    Debug.Log(destination);
    System.IO.File.WriteAllText(destination, data);
  }

  public void LoadFile()
  {
    string destination = Application.persistentDataPath + "/save.txt";
    var data = System.IO.File.ReadAllText(destination);
    Debug.Log(data);
    var objDict = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, object>>>(data);

    Debug.Log(string.Join(System.Environment.NewLine, objDict));
    foreach (var kv in objDict)
    {
      var go = GameObject.Find(kv.Key);
      if (go != null)
      {
        var allSaveable = Interfaces.GetInterfaces<ISaveable>(go);
        foreach (var saveable in allSaveable)
        {
          saveable.FromJsonSaveData(kv.Value);
        }
      }
    }
  }

  public void DoLoad()
  {
    Debug.Log("Loading...");

    LoadFile();

    // var go = GameObject.Find(name);
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
