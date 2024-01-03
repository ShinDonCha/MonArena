using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

//닉네임 변경 판넬 스크립트
//NickPanel 프리팹에 붙여서 사용
public class NickPanelCtrl : MonoBehaviour, IButtonClick
{
    [Header("----------- 게임 오브젝트 연결 -----------")]
    [SerializeField]
    private InputField nickIFd = null;                             //닉네임 InputField
    public string GetReqNick { get { return nickIFd.text; } }      //UserPanelCtrl의 NetworkMgr에서 닉네임 저장 시 닉네임 InputField의 텍스트를 받아가기 위한 프로퍼티

    [SerializeField]
    private Text errorText = null;                //에러문구 표시용 텍스트
    private readonly float errorTime = 2.0f;      //에러문구 표시 시간

    // Start is called before the first frame update
    void Start()
    {
        errorText.enabled = false;      //시작 시 에러텍스트 꺼진상태로 시작
        nickIFd.Select();               //시작 시 커서를 닉네임InputField에 놓기
    }

    public void ButtonOnClick(Button PushBtn)       //Hierarchy의 버튼 오브젝트들의 OnClick()을 통해 실행
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.OKButton:           //변경버튼 클릭 시 닉네임 변경 실행
                if (nickIFd.text.Length <= 0)   //닉네임을 입력하지 않았으면
                    StartCoroutine(ErrorOnOff("닉네임은 최소 1글자 이상이어야 합니다."));
                else if (nickIFd.text.Length > 6)   //닉네임이 6글자를 초과했으면
                    StartCoroutine(ErrorOnOff("닉네임은 최대 6글자만 가능합니다."));
                else  //이상 없으면
                    GetComponentInParent<UserPanelCtrl>().ReqNickCheck();   //닉네임 확인 요청 함수 호출
                break;

            case ButtonList.CancelButton:
                Destroy(gameObject);   //취소버튼 클릭 시 게임오브젝트 파괴
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }

    public IEnumerator ErrorOnOff(string Str)     //에러 문구 온오프 코루틴 (이 스크립트와 NetworkMgr의 NetworkMgr의 NickChangeCo 코루틴에서 사용)
    {
        errorText.text = Str;
        errorText.enabled = true;
        yield return new WaitForSecondsRealtime(errorTime);
        errorText.enabled = false;
    }
}
