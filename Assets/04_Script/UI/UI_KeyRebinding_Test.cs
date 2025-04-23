using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_KeyRebinding_Test : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpAction = null;
    [SerializeField] private Player_Manager playerManager = null;
    [SerializeField] private GameObject startRebindingObject = null;
    [SerializeField] private GameObject waitingForInputObject = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Update()
    {
        if (playerManager == null) playerManager = GameObject.Find("Player(Clone)").transform.GetComponent<Player_Manager>();
    }

    public void StartRebinding()
    {
        startRebindingObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        playerManager.playerinput.SwitchCurrentActionMap("Menu");

        rebindingOperation = jumpAction.action.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnMatchWaitForAnother(0.1f).OnComplete(operation => RebindComplete()).Start();
    }

    private void RebindComplete()
    {
        rebindingOperation.Dispose();

        startRebindingObject.SetActive(true);
        waitingForInputObject.SetActive(false);

        playerManager.playerinput.SwitchCurrentActionMap("Player");
    }
}
