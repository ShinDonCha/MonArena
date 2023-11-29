using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//전투중 표시되는 몬스터 머리위의 HP바를 컨트롤 하기위한 스크립트
//몬스터 오브젝트 하위의 StateCanvas에 붙여서 사용
public class StCanvasCtrl : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);   //초기 Off상태
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.position;  //항상 카메라에 정면이 보이도록 하기
    }

    public void EffectSet(GameObject Prefab, float SusTime)  //이펙트 생성 코루틴 (이펙트 프리팹, 지속시간)
    {
        GameObject a_GO = Instantiate(Prefab, transform);    //이펙트 생성
        Destroy(a_GO, SusTime);     //지속시간 후 삭제
    }
}
