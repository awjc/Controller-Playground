using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
  }

  public void DoLoad()
  {
    Debug.Log("Loading");
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
