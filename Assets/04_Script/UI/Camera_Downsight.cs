using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Camera_Downsight : MonoBehaviour
{
    private GameManager manager;
    private PlayerInput playerInput;
    private Player_Health_Manager healthManager;
    private float viewTimer = 0;
    [SerializeField] private float viewTime;
    [SerializeField] private float cameraMoveTime;
    [SerializeField] private float verticalValue;

    private bool lookdown = false;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        healthManager = transform.GetComponent<Player_Health_Manager>();
        playerInput = transform.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (!manager.Active_Dialogue)
        {
            if (!manager.PlayerDie && !manager.playerMove)
            {
                if (lookdown)
                {
                    viewTimer += Time.deltaTime;
                }
                else if (manager.camMoveCheck && !lookdown)
                {
                    viewTimer = 0;
                    manager.camMoveCheck = false;
                    healthManager.Get_CurrentCam().GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY += 0.2f;
                }
            }
            else
            {
                if (manager.camMoveCheck)
                {
                    viewTimer = 0;
                    manager.camMoveCheck = false;
                    healthManager.Get_CurrentCam().GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY += 0.2f;
                }
            }
        }

        if (viewTimer >= viewTime)
        {
            if (!manager.camMoveCheck)
            {
                manager.camMoveCheck = true;
                healthManager.Get_CurrentCam().GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY -= 0.2f;
            }
        }
    }


    public void ActionLookDown(InputAction.CallbackContext context)
    {
        if (context.performed) lookdown = true;
        else if (context.canceled) lookdown = false;
    }
}
