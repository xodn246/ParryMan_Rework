using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_SetCurrnetControlScheme : MonoBehaviour
{
    PlayerInput playerInput;

    private void Start()
    {
        playerInput = transform.GetComponent<PlayerInput>();
    }

    public void OnControlsChanged()
    {
        GameManager.instance.Set_CurrentControlScheme(playerInput.currentControlScheme);
        Debug.Log("ControlChanged : " + playerInput.currentControlScheme);
    }
}
