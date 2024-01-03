using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//상태변화(버프&디버프), 대미지 등을 표시해줄 텍스트의 위치설정 및 애니메이션 실행을 위한 스크립트
//BuffTxtRoot 프리팹에 붙여서 사용
public class BuffTextRootCtrl : MonoBehaviour
{
    [SerializeField]
    private Text buffText = null;       //하위의 BuffText 연결
    [SerializeField]
    private Text deBuffText = null;     //하위의 DeBuffText 연결

    private Animation Anim = null;      //애니메이션 컴포넌트를 담을 변수
    private readonly Queue<TakeAct> requestTA = new();    //요청받은 상태변화 종류를 담기위한 큐

    private void Awake()
    {
        Anim = GetComponent<Animation>();       //이 오브젝트의 애니메이션 컴포넌트 가져오기
    }

    public void AnimPlay(Vector3 ObjPos, TxtAnimList AnimList, TakeAct Act)  //텍스트 애니메이션 재생 함수 (상위의 TextArea 게임오브젝트의 TextAreaCtrl 스크립트에서 실행)
    {
        Vector3 a_Vt3 = new (ObjPos.x, ObjPos.y + 1.5f, ObjPos.z);          //텍스트 생성할 World상의 위치 
        transform.position = Camera.main.WorldToScreenPoint(a_Vt3);         //World -> Screen으로 변경 후 적용

        Anim.CrossFadeQueued(AnimList.ToString(), 0.0f);   //실행중인 애니메이션이 종료 된 후 실행요청
        requestTA.Enqueue(Act);                            //어떤 상태변화를 요청받았는지 저장
    }

    public void TextSet(TxtAnimList AnimList)       //요청받은 TakeAct에 따라 출력할 텍스트를 변경하는 함수(BuffTxtAnim과 DeBuffTxtAnim에서 이벤트로 실행, 실행중인 애니메이션과 일치하는 매개변수 전달)
    {
        if (requestTA.Count == 0)
            return;

        string a_ActStr = requestTA.Dequeue() switch  //실행 해야할 효과에 따라 텍스트 변경
        {
            TakeAct.Dmg => "공격력",
            TakeAct.ASpd => "공격속도",
            TakeAct.Defence => "방어력",
            TakeAct.MDefence => "마법저항력",
            _ => ""
        };

        switch(AnimList)
        {
            case TxtAnimList.BuffTxtAnim:       //버프 애니메이션을 재생 중이라면
                buffText.text = a_ActStr + " 증가";
                break;

            case TxtAnimList.DeBuffTxtAnim:     //디버프 애니메이션을 재생 중이라면
                deBuffText.text = a_ActStr + " 감소";
                break;
        }
    }
}
