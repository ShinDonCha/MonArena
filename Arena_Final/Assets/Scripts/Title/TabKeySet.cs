using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Tab키를 통해 다음 오브젝트를 선택할 수 있도록 하기위한 스크립트
//TitleScene의 LoginPanel, SignupPanel에 붙여서 사용
public class TabKeySet : MonoBehaviour
{
    [SerializeField]
    private InputField IDInputField = null;     //IDInputField
    [SerializeField]
    private InputField PWInputField = null;     //PWInputField

    private void OnEnable()
    {
        IDInputField.text = "";     //입력창 초기화
        PWInputField.text = "";     //입력창 초기화

        if (IDInputField != null)
            IDInputField.Select();      //IDInputField 선택 상태로 변경
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))     //Tab키 클릭시 네비게이션을 따라 다음 위치로 커서 이동
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().Select();
    }
}
