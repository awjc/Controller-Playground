using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
    Debug.Log("Saving");

    GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
    var allSaveable = Interfaces.GetAllInterfaces<ISaveable>(allObjects);

    foreach (ISaveable savObj in allSaveable)
    {
      Debug.Log(string.Format("{0} : {1}", savObj.UniqueNameId(), savObj.ToJsonSaveData()));
    }

    Debug.Log(allObjects);
    Debug.Log(allSaveable);

    SaveFile("ABCDEFG This is a test data");
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
  }

  public void DoLoad()
  {
    Debug.Log("Loading");

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
