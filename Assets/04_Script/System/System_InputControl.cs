using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class System_InputControl : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = transform.GetComponent<PlayerInput>();
    }

    public void OnControlSchemeChanged(string _controlScheme)
    {
        Debug.Log($"OnControlSchemeChanged : {_controlScheme}");
    }
}
