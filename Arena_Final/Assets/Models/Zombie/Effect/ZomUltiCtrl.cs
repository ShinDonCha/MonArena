using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie의 궁극기 컨트롤 스크립트
//ZomUltimate에 붙여서 사용
public class ZomUltiCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject damageDummy = null;   //범위내 적에게 대미지를 주는 더미 (하위의 ZomUltiDummy)

    private void Awake()
    {
        tag = transform.parent.tag;         //태그 변경
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DamageCo());     //대미지 코루틴 실행
    }

    private IEnumerator DamageCo()      //0.5초마다 대미지를 주기위한 코루틴
    {
        damageDummy.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damageDummy.SetActive(false);
        yield return new WaitForSeconds(0.4f);        
        StartCoroutine(DamageCo());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();        //파괴될때 모든 코루틴 종료
    }
}
