using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//유저의 몬스터의 상태표시창을 컨트롤하기위한 스크립트
//InGame와 RankGame의 MonStatus에 붙여서 사용
public class MonStatusCtrl : MonoBehaviour
{
    [SerializeField]
    private MonStorage monStore = null;     //이미지, 오브젝트 저장소
    [SerializeField]
    private Image monImg = null;            //하위의 몬스터 이미지 컴포넌트
    [SerializeField]
    private Transform starLineTr = null;    //성급에 맞게 별이미지를 보여주기 위한 Transform
    [SerializeField]
    private Image hpBarImg = null;          //하위의 HP바 이미지 컴포넌트
    [SerializeField]
    private Image ultiBarImg = null;        //하위의 궁극기바 이미지 컴포넌트

    private CmnMonCtrl curPointCMC = null;  //이 오브젝트의 자식번호와 일치하는 Point에 놓여있는 몬스터의 CmnMonCtrl을 담을 변수
    private Color32 deathColor = new(80, 80, 80, 255);      //죽었을 때 바꿔줄 색깔
    private bool isDie = false;             //현재 이 오브젝트와 연결된 몬스터가 죽었는지 나타내기 위한 변수

    private void OnEnable()     //Mgr스크립트에서 CombatUI가 켜질 때 실행
    {
        curPointCMC = FindClass.CurSetPoint[transform.GetSiblingIndex()].GetComponentInChildren<CmnMonCtrl>();  //이 오브젝트의 자식번호와 일치하는 Point에 놓여있는 몬스터의 CmnMonCtrl을 받아오기

        if (curPointCMC != null)    //해당 Point에 몬스터가 있다면
        {
            monImg.sprite = monStore.monstersImg[(int)curPointCMC.CMCmonStat.monName];  //몬스터 정보에 맞게 이미지 변경

            for (int i = 0; i < curPointCMC.CMCmonStat.starForce; i++)                  //몬스터의 성급에 맞게 별이미지 켜기
                starLineTr.GetChild(i).gameObject.SetActive(true);
        }
        else                        //해당 Point에 몬스터가 없다면
            gameObject.SetActive(false);    //이 오브젝트 끄기
    }

    // Update is called once per frame
    void Update()
    {
        if (isDie)      //이 오브젝트와 연결된 몬스터가 죽은상태면 취소
            return;

        if (curPointCMC.gameObject.activeSelf)    //해당 몬스터가 살아있다면
        {
            hpBarImg.fillAmount = (float)curPointCMC.CMCcurStat.hp / curPointCMC.CMCmonStat.hp;
            ultiBarImg.fillAmount = (float)curPointCMC.UltiGage / 100;
        }
        else    //해당 몬스터가 죽었다면
        {
            foreach (Image Img in GetComponentsInChildren<Image>())
                Img.color = deathColor;

            isDie = true;       //죽은 상태로 변경
        }
    }
}
