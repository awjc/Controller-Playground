using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaverLoader : MonoBehaviour
{

  public InputActionAsset primaryActions;

  private InputActionMap gameplayActionMap;

  void Start()
  {
    gameplayActionMap = primaryActions.FindActionMap("SaveLoad", true);
    if (gameplayActionMap == null) { return; }

    var saveAction = gameplayActionMap.FindAction("Save");
    saveAction.performed += context => DoSave();

    var loadAction = gameplayActionMap.FindAction("Load");
    loadAction.performed += context => DoLoad();
  }

  public void DoSave()
  {
    Debug.Log("Saving");
  }

  public void DoLoad()
  {
    Debug.Log("Loading");
  }
}
