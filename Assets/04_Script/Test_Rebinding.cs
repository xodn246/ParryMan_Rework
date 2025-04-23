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
            Debug.Log("���ε� �� " + binding);
        }

        playerInput.SwitchCurrentActionMap("KeyBind");

        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                Debug.Log("Ű��");
                Debug.Log("�ƹ�Ű�� �Է�");
                rebindingOperation = currentAction.action.PerformInteractiveRebinding(0)
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(operation => RebindComplete())
                    .Start();
                break;

            default:
                Debug.Log("�ٸ��� �Էµ�");
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
            Debug.Log("���ε� �� " + binding);
        }

        Debug.Log("�缳�� �Ϸ�");
        playerInput.SwitchCurrentActionMap("Player");
        rebindingOperation.Dispose();
        currentAction.action.Enable();
    }
}