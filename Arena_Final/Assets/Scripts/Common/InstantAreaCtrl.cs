using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//투사체, 스킬이펙트 등의 오브젝트들을 생성할 Transform을 제공하기 위한 스크립트
//각 생성자의 태그에 맞는 AreaTr의 하위로 오브젝트가 생성되고, 생성된 오브젝트들은 Area의 태그를 통해 부모의 태그를 구분할 수 있다.
//전투가 일어나는 씬의 InstantArea 게임오브젝트에 붙여서 사용
public class InstantAreaCtrl : MonoBehaviour
{
    [SerializeField]
    private Transform playerAreaTr = null;   //Ally의 태그를 가진 Area
    [SerializeField]
    private Transform enemyAreaTr = null;    //Enemy의 태그를 가진 Area
    [SerializeField]
    private Transform team1AreaTr = null;    //Team1의 태그를 가진 Area
    [SerializeField]
    private Transform team2AreaTr = null;    //Team2의 태그를 가진 Area

    // Start is called before the first frame update
    void Start()
    {
        FindClass.AreaTrFunc = GetAreaTr;       //외부에서 Area의 Transform을 찾을 수 있도록 정적으로 생성한 Func에 등록
    }

    private Transform GetAreaTr(string TeamTag)     //태그에 맞는 Area를 찾는 함수
    {
        if (!Enum.TryParse(TeamTag, out Team Tm))
            return null;

        switch(Tm)
        {
            case Team.Ally:
                return playerAreaTr;

            case Team.Enemy:
                return enemyAreaTr;

            case Team.Team1:
                return team1AreaTr;

            case Team.Team2:
                return team2AreaTr;

            default:
                return null;
        }
    }
}
