using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//몬스터 점수판에 알맞은 정보를 넣기위한 스크립트
//MonPart 프리팹에 붙여서 사용
public class MonPartCtrl : MonoBehaviour
{
    //--------- 게임오브젝트 연결
    [SerializeField]
    private MonStorage monStore = null;     //이미지, 오브젝트 저장소
    [SerializeField]
    private Image monImg = null;            //몬스터 이미지
    [SerializeField]
    private Image dmgBarImg = null;         //대미지 바 이미지
    [SerializeField]
    private Text dmgBarText = null;         //대미지 바 텍스트
    [SerializeField]
    private Image HPBarImg = null;          //HP 바 이미지
    [SerializeField]
    private Text HPBarText = null;          //HP 바 텍스트
    //--------- 게임오브젝트 연결

    // Start is called before the first frame update
    void Start()
    {
        ResultBoardCtrl a_RBC = GetComponentInParent<ResultBoardCtrl>();    //부모의 스크립트 가져오기

        if (a_RBC == null)
            return;

        int a_Num = transform.GetSiblingIndex();    //이 MonPart프리팹의 자식번호

        monImg.sprite = monStore.monstersImg[a_RBC.CmnListNameNum[a_Num]];                        //몬스터 이미지 설정
        dmgBarImg.fillAmount = (float)a_RBC.CmnListDmg[a_Num] / a_RBC.TotalMonDmg;                //대미지 바 수치 설정
        dmgBarText.text = string.Format("{0} / {1}", a_RBC.CmnListDmg[a_Num], a_RBC.TotalMonDmg); //대미지 바 텍스트 설정
        HPBarImg.fillAmount = (float)a_RBC.CmnListHP[a_Num] / a_RBC.TotalMonHP;                   //HP 바 수치 설정
        HPBarText.text = string.Format("{0} / {1}", a_RBC.CmnListHP[a_Num], a_RBC.TotalMonHP);    //HP바 텍스트 설정
    }
}
