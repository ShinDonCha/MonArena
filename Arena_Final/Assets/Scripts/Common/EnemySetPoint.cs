using UnityEngine;
using UnityEngine.SceneManagement;

//적 몬스터들이 배치하는 곳인 EnemySetPoints오브젝트를 컨트롤 하기 위한 스크립트
//InGameScene과 RankGameScene의 EnemySetPoints 게임오브젝트에 붙여 사용
public class EnemySetPoint : MonoBehaviour
{
    [SerializeField]
    private MonStorage monStore = null;    //몬스터 이미지, 오브젝트 저장소

    private Transform[] PointsTr;          //EnemySetPoints 하위의 Point들의 transform

    private void Awake()
    {
        PointsTr = new Transform[transform.childCount];     //Point 수 만큼 배열 생성

        for (int i = 0; i < transform.childCount; i++)      //Point의 순서에 맞게 transform 저장
            PointsTr[i] = transform.GetChild(i);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains(SceneList.InGameScene.ToString()))
            StageSetMonster();       //스테이지에 따른 몬스터 배치
        else
            RankSetMonster();       //랭크 상대에 따른 몬스터 배치
    }

    private void StageSetMonster()       //유저의 스테이지에 맞는 몬스터 생성 함수(현재 임시로 5스테이지까지만 구성)
    {
        int a_RepeatNum = PlayerInfo.CombatStage < 5 ? PlayerInfo.CombatStage : 5;

        for (int i = 0; i < a_RepeatNum; i++)
            Instantiate(monStore.monstersObj[i], PointsTr[i]).tag = Team.Enemy.ToString();  //몬스터 생성
    }

    private void RankSetMonster()       //랭커의 덱 정보에 맞는 몬스터 생성 함수
    {
        for(int i = 0; i < FindClass.RankDName.Length; i++)
        {
            if (FindClass.RankDName[i].Equals(MonsterName.None))        //비어있는 곳이면 넘어가기
                continue;

            Instantiate(monStore.monstersObj[(int)FindClass.RankDName[i]], PointsTr[i]).tag = Team.Enemy.ToString();        //몬스터 생성
        }
    }
}
