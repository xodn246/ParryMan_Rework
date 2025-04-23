using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class UI_TitleMenu : MonoBehaviour, IPointerEnterHandler
{
    private DataManager dataManager;

    [SerializeField] private Object_SoundManager soundManager;
    [SerializeField] private EventSystem eventSystem; // 이벤트 시스템 직접 넣어주기

    [SerializeField] private List<Button> buttons; //버튼 네비게이션 변경시 사용할 버튼 리스트
    [SerializeField] private List<GameObject> indicator;

    private Input_Player playerInput;



    private void Awake()
    {
        dataManager = GameObject.FindObjectOfType<DataManager>().GetComponent<DataManager>(); //밑에 기능 추가할때 쓰려고 Datamanger 불러둠
    }

    private void Start()
    {
        SetContinueActivate();
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);

        playerInput = new Input_Player();
        playerInput.Menu.Enable();
    }

    private void Update()
    {
        Set_SelectObject_Input_Vertical();
        PrintIndicator();


    }

    public void OnPointerEnter(PointerEventData eventData) //마우스 올린 오브젝트를 선택된 오브젝트로 설정해주는 함수
    {
        eventData.pointerCurrentRaycast.gameObject.GetComponent<Button>().Select();

        if (GameManager.instance.Get_LastSelectedUI() != null && GameManager.instance.Get_LastSelectedUI() != eventSystem.currentSelectedGameObject) SoundManager.instance.SFXPlayer_UI(soundManager.Get_AudioClip("Select"), gameObject.transform);
        GameManager.instance.Set_LastSelectedUI(eventData.pointerCurrentRaycast.gameObject);
    }

    private void Set_SelectObject_Input_Vertical()
    {
        if (playerInput.Menu.Navigate.ReadValue<Vector2>() != Vector2.zero) // 키보드 입력시 이벤트시스템에 선택된 오브젝트가 없을경우 가장 최근 선택되었던 오브젝트 넣어주기
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(GameManager.instance.Get_LastSelectedUI());
            }

            if (GameManager.instance.Get_LastSelectedUI() != null && GameManager.instance.Get_LastSelectedUI() != eventSystem.currentSelectedGameObject) SoundManager.instance.SFXPlayer_UI(soundManager.Get_AudioClip("Select"), gameObject.transform);
            GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
        }
    }


    public void SetContinueActivate()
    {
        // 저장된 정보가 존재하지 않으면 continue 버튼을 비활성화 시키며 네비게이션 변경
        if (dataManager.nowData.sceneName == "")
        {
            Navigation newNav01 = new Navigation();
            Navigation newNav02 = new Navigation();

            newNav01.mode = Navigation.Mode.Explicit;
            newNav02.mode = Navigation.Mode.Explicit;

            newNav01.selectOnUp = buttons[3];
            newNav01.selectOnDown = buttons[2];
            newNav02.selectOnUp = buttons[0];
            newNav02.selectOnDown = buttons[3];

            buttons[0].navigation = newNav01;
            buttons[2].navigation = newNav02;
        }
        else
        {
            Navigation newNav01 = new Navigation();
            Navigation newNav02 = new Navigation();

            newNav01.mode = Navigation.Mode.Explicit;
            newNav02.mode = Navigation.Mode.Explicit;

            newNav01.selectOnUp = buttons[3];
            newNav01.selectOnDown = buttons[1];
            newNav02.selectOnUp = buttons[1];
            newNav02.selectOnDown = buttons[3];

            buttons[0].navigation = newNav01;
            buttons[2].navigation = newNav02;
        }
    }

    public void PrintIndicator()
    {
        switch (GameManager.instance.Get_LastSelectedUI_Name())
        {
            case null:
                for (int i = 0; i < indicator.Count; i++)
                {
                    indicator[i].SetActive(false);
                }
                break;

            case "Button_start":
                indicator[0].SetActive(true);
                indicator[1].SetActive(false);
                indicator[2].SetActive(false);
                indicator[3].SetActive(false);
                break;

            case "Button_continue":
                indicator[0].SetActive(false);
                indicator[1].SetActive(true);
                indicator[2].SetActive(false);
                indicator[3].SetActive(false);
                break;

            case "Button_option":
                indicator[0].SetActive(false);
                indicator[1].SetActive(false);
                indicator[2].SetActive(true);
                indicator[3].SetActive(false);
                break;

            case "Button_exit":
                indicator[0].SetActive(false);
                indicator[1].SetActive(false);
                indicator[2].SetActive(false);
                indicator[3].SetActive(true);
                break;
        }
    }

    public void Select_Sound_Function()
    {
        SoundManager.instance.SFXPlayer_UI(soundManager.Get_AudioClip("Choice"), gameObject.transform);
    }
}