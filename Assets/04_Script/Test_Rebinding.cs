using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Test_Rebinding : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference currentAction = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Update()
    {
        if (playerInput == null) playerInput = GameObject.FindObjectOfType<PlayerInput>().GetComponent<PlayerInput>();
    }

    public void Start_Rebinding()
    {
        foreach (var binding in currentAction.action.bindings)
        {
            Debug.Log("바인딩 전 " + binding);
        }

        playerInput.SwitchCurrentActionMap("KeyBind");

        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                Debug.Log("키마");
                Debug.Log("아무키나 입력");
                rebindingOperation = currentAction.action.PerformInteractiveRebinding(0)
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(operation => RebindComplete())
                    .Start();
                break;

            default:
                Debug.Log("다른거 입력됨");
                break;
        }
    }

    private void RebindCancel()
    {
    }

    private void RebindComplete()
    {
        foreach (var binding in currentAction.action.bindings)
        {
            Debug.Log("바인딩 후 " + binding);
        }

        Debug.Log("재설정 완료");
        playerInput.SwitchCurrentActionMap("Player");
        rebindingOperation.Dispose();
        currentAction.action.Enable();
    }
}