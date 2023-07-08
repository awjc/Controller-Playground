using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubeController : MonoBehaviour
{
  public InputActionAsset primaryActions;

  private InputActionMap gameplayActionMap;

  private List<InputAction> allActions;

  private Rigidbody rb;

  // Start is called before the first frame update
  void Start()
  {
    gameplayActionMap = primaryActions.FindActionMap("Gameplay", true);
    if (gameplayActionMap == null) { return; }

    var rotate1Action = gameplayActionMap.FindAction("Rotate 1");
    rotate1Action.performed += context => HandleRotate1();

    var rotate2Action = gameplayActionMap.FindAction("Rotate 2");
    rotate2Action.performed += context => HandleRotate2();

    var rotate3Action = gameplayActionMap.FindAction("Rotate 3");
    rotate3Action.performed += context => HandleRotate3();

    var rotate4Action = gameplayActionMap.FindAction("Rotate 4");
    rotate4Action.performed += context => HandleRotate4();

    allActions = new List<InputAction> {
     rotate1Action, rotate2Action, rotate3Action, rotate4Action
    };

    Debug.Log("AWJC control registered");

    rb = GetComponent<Rigidbody>();
  }

  protected void Update()
  {
    // Debug.Log($"GPC: {Gamepad.current.leftStick.ReadValue()}.");
    // Debug.Log($"2: {Gamepad.current.buttonSouth.ReadValue()}");

    // // For a button type action.
    // if (rotate1Action.triggered) {
    //     Debug.Log("297836872968732468723648723647826378462873");
    // } else if (false) {
    //     Debug.Log("$$$$$$$$$$$$$$@@@@@@");
    // } else {
    //     Debug.Log("no");
    // }
    /* ... */
    ;

    // For a value type action.
    // (Vector2 is just an example; pick the value type that is the right
    // one according to the bindings you have)
    // var v = action.ReadValue<Vector2>();
  }

  void HandleRotate1()
  {
    Debug.Log("AWJC HANDLE ROTATE 1");
    rb.AddTorque(Vector3.right * 5, ForceMode.Force);
  }

  void HandleRotate2()
  {
    Debug.Log("AWJC HANDLE ROTATE 2");
    rb.AddTorque(Vector3.left * 5, ForceMode.Force);
  }

  void HandleRotate3()
  {
    Debug.Log("AWJC HANDLE ROTATE 3");
    rb.AddTorque(Vector3.up * 5, ForceMode.Force);
  }

  void HandleRotate4()
  {
    Debug.Log("AWJC HANDLE ROTATE 4");
    rb.AddTorque(Vector3.down * 5, ForceMode.Force);
  }

  private void OnEnable()
  {
    primaryActions?.Enable();
    if (allActions != null)
    {
      foreach (var ia in allActions)
      {
        ia?.Enable();
      }
    }
  }

  private void OnDisable()
  {
    primaryActions?.Disable();
    if (allActions != null)
    {
      foreach (var ia in allActions)
      {
        ia?.Disable();
      }
    }
  }
}
