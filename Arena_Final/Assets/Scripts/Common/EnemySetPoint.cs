using UnityEngine;
using UnityEngine.SceneManagement;

//�� ���͵��� ��ġ�ϴ� ���� EnemySetPoints������Ʈ�� ��Ʈ�� �ϱ� ���� ��ũ��Ʈ
//InGameScene�� RankGameScene�� EnemySetPoints ���ӿ�����Ʈ�� �ٿ� ���
public class EnemySetPoint : MonoBehaviour
{
    [SerializeField]
    private MonStorage monStore = null;    //���� �̹���, ������Ʈ �����

    private Transform[] PointsTr;          //EnemySetPoints ������ Point���� transform

    private void Awake()
    {
        PointsTr = new Transform[transform.childCount];     //Point �� ��ŭ �迭 ����

        for (int i = 0; i < transform.childCount; i++)      //Point�� ������ �°� transform ����
            PointsTr[i] = transform.GetChild(i);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains(SceneList.InGameScene.ToString()))
            StageSetMonster();       //���������� ���� ���� ��ġ
        else
            RankSetMonster();       //��ũ ��뿡 ���� ���� ��ġ
    }

    private void StageSetMonster()       //������ ���������� �´� ���� ���� �Լ�(���� �ӽ÷� 5�������������� ����)
    {
        int a_RepeatNum = PlayerInfo.CombatStage < 5 ? PlayerInfo.CombatStage : 5;

        for (int i = 0; i < a_RepeatNum; i++)
            Instantiate(monStore.monstersObj[i], PointsTr[i]).tag = Team.Enemy.ToString();  //���� ����
    }

    private void RankSetMonster()       //��Ŀ�� �� ������ �´� ���� ���� �Լ�
    {
        for(int i = 0; i < FindClass.RankDName.Length; i++)
        {
            if (FindClass.RankDName[i].Equals(MonsterName.None))        //����ִ� ���̸� �Ѿ��
                continue;

            Instantiate(monStore.monstersObj[(int)FindClass.RankDName[i]], PointsTr[i]).tag = Team.Enemy.ToString();        //���� ����
        }
    }
}
