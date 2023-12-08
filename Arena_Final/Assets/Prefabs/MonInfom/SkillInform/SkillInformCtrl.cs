using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//��ų ������ Ŭ�� �� ���õ� ��ų ������ ���޿� ���� ������ �����ֱ� ���� ��ũ��Ʈ
//SkillInform �����տ� �ٿ��� ���
public class SkillInformCtrl : MonoBehaviour, IPointerClickHandler
{
    [Header("----------- ���� ������Ʈ ���� -----------")]
    [SerializeField]
    private Text nameText = null;       //��ų �̸��� ������ �ؽ�Ʈ
    [SerializeField]
    private Text explainText = null;    //��ų ������ ������ �ؽ�Ʈ
    [SerializeField]
    private Transform starTab = null;   //�� ��ư���� ����ִ� �� ��

    private MonsterStat monStat;       //���� ������ ������ �޾ƿ��� ���� ����
    private ExplainList CurExList;     //SkillGroup���� ���õ� ��ų�� ������� �˱� ���� ����

    private Color32 defaultColor = new(255, 255, 255, 255);     //�⺻ ��ư�� ����
    private Color32 selectColor = new(130, 130, 130, 255);      //���õ� ��ư�� ����

    private readonly string explainfmt1 = "{0}\n<color=#0D47A1>{1}</color>\n\n";  //��ų ����(���� ����)�� ǥ���� ���ڿ� ����
    private readonly string explainfmt2 = "<color=#FF0000>{0} :</color> {1}"; //��ų ����(��Ÿ��, ���� ��)�� ǥ���� ���ڿ� ����

    private void Awake()
    {
        monStat = PlayerInfo.MonList[FindClass.MISelNum];    //���� �����ִ� ������ ���� �޾ƿ���
        CurExList = (ExplainList)FindClass.SISelNum;         //���õ� ��ų��ȣ�� Enum���� ����   
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMonEx(monStat.starForce);    //ó���� ���� ������ ���޿� �´� ��ų���� ���
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.name.Contains(name))       //����â�� �ƴ� �ٱ��� Ŭ���ϸ� ����â ����
        {
            EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
            Destroy(transform.parent.gameObject);
        }
    }

    public void SetMonEx(int StarF)       //������ ����ư�� ���󺯰�, ��û���� ��ų���� ����Ʈ�� ���� �� ������ ����ϴ� �Լ� (StartButton���� OnClick()���� �Ű������� ��ư�� ��ȣ�� �޾ƿ´�)
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���

        //--------------- ������ ���������� ���� ���� ���� ---------------
        for (int i = 0; i < starTab.childCount; i++)      //�� �� ��ü�� ����
        {
            if (i.Equals(StarF))        
                starTab.GetChild(i).GetComponent<Image>().color = selectColor;
            else
                starTab.GetChild(i).GetComponent<Image>().color = defaultColor;
        }
        //--------------- ������ ���������� ���� ���� ���� ---------------

        switch (monStat.monName)
        {
            case MonsterName.Soldier:
                Soldier a_SoldierCtrl = MonInformMgr.Instance.GetComponentInChildren<Soldier>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>������</color>";
                        explainText.text = string.Format(explainfmt1, "��ǥ�� �����Ͽ� �ٹ̻����� �����Ѵ�.", "�ٹ̻����� ���� �ε��� �����Ͽ� �ֺ��� �ִ� ���鿡�� ���ظ� �ش�.") +
                                            string.Format(explainfmt2, "����", "���߷κ��� " + a_SoldierCtrl.SolUltiRadius + " �Ÿ�\n") +
                                            string.Format(explainfmt2, "�����", a_SoldierCtrl.GetUltiDmg(StarF));
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "�๰ ����";
                        explainText.text = string.Format(explainfmt1, "�ڽſ��� �๰�� �����Ͽ� ������縦 Ȱ��ȭ�Ѵ�.", "�ñر� �������� �����ǰ� �� �ݵ����� �������� HP�� �����Ѵ�.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", a_SoldierCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n") +
                                           string.Format(explainfmt2, "������ ������", a_SoldierCtrl.SolSk1UltiG + "\n") +
                                           string.Format(explainfmt2, "����", "�ڱ� �ڽ�\n") +
                                           string.Format(explainfmt2, "��Ÿ��", a_SoldierCtrl.SkSet.Cool[(int)Skill.Sk1] + "��");
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "������û";
                        explainText.text = string.Format(explainfmt1, "��������� ���� ��� 4�븦 ȣ���Ѵ�.", "����� �������� �̵��ϸ� ���� �߰��ϸ� ������ ���Ѵ�.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", a_SoldierCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n") +
                                           string.Format(explainfmt2, "����", "�������� �ִ� ������ ����\n") +
                                           string.Format(explainfmt2, "��� ���ӽð�", a_SoldierCtrl.DronLifeT + "��\n") +
                                           string.Format(explainfmt2, "��� ���ݼӵ�", "�ʴ� " + 1 / a_SoldierCtrl.DronASpd + "��\n") +
                                           string.Format(explainfmt2, "��� �����", "�ߴ� " + a_SoldierCtrl.GetDronDmg(StarF) + "\n") +
                                           string.Format(explainfmt2, "��Ÿ��", a_SoldierCtrl.SkSet.Cool[(int)Skill.Sk2] + "��");
                        break;
                }
                break;

            case MonsterName.Zombie:
                Zombie a_ZombieCtrl = MonInformMgr.Instance.GetComponentInChildren<Zombie>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>������ ����</color>";
                        explainText.text = string.Format(explainfmt1, "�ǿ� ���� ������ �������� �����κ��� ������� ���Ѵ´�.", "���� ���� ���鿡�� ���ظ� �ְ� ���ط��� �Ϻθ� �ڽ��� ü������ ��ȯ�Ѵ�.") +
                                           string.Format(explainfmt2, "���ӽð�", a_ZombieCtrl.ZomUltiTime + "��\n") +
                                           string.Format(explainfmt2, "����", "�ڽ����κ��� " + a_ZombieCtrl.ZomUltiRadius + " �Ÿ�\n") +
                                           string.Format(explainfmt2, "ü����ȯ��", a_ZombieCtrl.ZomUltiRatio * 100 + "%\n") +
                                           string.Format(explainfmt2, "�����", "�ʴ� " + a_ZombieCtrl.GetUltiDmg(StarF));
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "����";
                        explainText.text = string.Format(explainfmt1, "���� ���� ������Ų��.", "������ ���� ���ظ� �԰� ���� �ð� ��ȭ�ȴ�.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", a_ZombieCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n") +
                                           string.Format(explainfmt2, "���ӽð�", a_ZombieCtrl.ZomSk1SusTime + "��\n") +
                                           string.Format(explainfmt2, "����", "���� ���� ���� ���\n") +
                                           string.Format(explainfmt2, "��/���� ���ҷ�", a_ZombieCtrl.GetSk1ReduVal(StarF) + "\n") +
                                           string.Format(explainfmt2, "�����", a_ZombieCtrl.GetSk1Dmg(StarF) + "\n") +
                                           string.Format(explainfmt2, "��Ÿ��", a_ZombieCtrl.SkSet.Cool[(int)Skill.Sk1] + "��");
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "��ȿ";
                        explainText.text = string.Format(explainfmt1, "������ ������ ������ ���Ǹ� ����.", "���� �Ÿ� �ȿ� �ִ� ���� ������ ���ݷ��� ���ҽ�Ű�� �ڽ��� �����ϰ� �Ѵ�.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", a_ZombieCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n") +
                                           string.Format(explainfmt2, "���ӽð�", a_ZombieCtrl.ZomSk2SusTime + "��\n") +
                                           string.Format(explainfmt2, "����", "�ڽ����κ��� " + a_ZombieCtrl.ZomSk2Radius + " �Ÿ�\n") +
                                           string.Format(explainfmt2, "���ݷ� ������", a_ZombieCtrl.GetReduceRatio(StarF) * 100 + "%\n") +
                                           string.Format(explainfmt2, "��Ÿ��", a_ZombieCtrl.SkSet.Cool[(int)Skill.Sk2] + "��");
                        break;
                }
                break;

            case MonsterName.GamblerCat:
                GamblerCat a_GamblerCatCtrl = MonInformMgr.Instance.GetComponentInChildren<GamblerCat>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>���ȸ��</color>";
                        explainText.text = string.Format(explainfmt1, "���⿡ ó�� �Ʊ��� ���س���.", "���� ���� ���� �� �Ʊ��� ü���� ũ�� ȸ����Ų��.") +
                                           string.Format(explainfmt2, "����", "�ڽ����κ��� " + a_GamblerCatCtrl.CatUltiRadius + " �Ÿ�\n") +
                                           string.Format(explainfmt2, "ȸ����", a_GamblerCatCtrl.GetUltiAmount(StarF));
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "ġ��";
                        explainText.text = string.Format(explainfmt1, "������ ����� �����Ͽ� �Ʊ��� ���´�.", "�Ʊ� �� ���� ü���� ȸ����Ų��.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", a_GamblerCatCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n") +
                                           string.Format(explainfmt2, "����", "���� ü���� ���� ���� �Ʊ� �� ��\n") +
                                           string.Format(explainfmt2, "ȸ����", a_GamblerCatCtrl.GetHealAmount(StarF) + "\n") +
                                           string.Format(explainfmt2, "��Ÿ��", a_GamblerCatCtrl.SkSet.Cool[(int)Skill.Sk1] + "��");
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "�޺���";
                        explainText.text = string.Format(explainfmt1, "������ ���ؾ��� �Ʊ��� ��⸦ �ø���.", "�Ʊ� �� ���� ������� ������Ų��.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", a_GamblerCatCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n") +
                                           string.Format(explainfmt2, "���ӽð�", a_GamblerCatCtrl.GetBuffTime(StarF) + "��\n") +
                                           string.Format(explainfmt2, "����", "���� ���ݷ��� ���� ���� �Ʊ� �� ��\n") +
                                           string.Format(explainfmt2, "����� ������", a_GamblerCatCtrl.CatSk2IncAmount * 100 + "%\n") +
                                           string.Format(explainfmt2, "��Ÿ��", a_GamblerCatCtrl.SkSet.Cool[(int)Skill.Sk2] + "��");
                        break;
                }
                break;

            case MonsterName.Mutant:
                Mutant a_MutantCtrl = MonInformMgr.Instance.GetComponentInChildren<Mutant>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>������</color>";
                        explainText.text = string.Format(explainfmt1, "�Ҹ����� ���� �ӵ��� �̵��ϸ� ��ī�ο� �������� ������ �������Ѵ�.", "���� ���� ���� ���鿡�� ���ظ� �ش�.") +
                                           string.Format(explainfmt2, "����", "�ڽ��� ����������� �ִ� " + a_MutantCtrl.MutUltiRange + " �Ÿ����� ��(������ �浹 �� ���� ����)\n") +
                                           string.Format(explainfmt2, "�����", a_MutantCtrl.GetUltiDmg(StarF));
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "�ϻ�";
                        explainText.text = string.Format(explainfmt1, "���� ȯ�濡 ������ ���� ���� �� ���� �޼Ҹ� �����Ѵ�.", "���� ���� �� ���� ���� ���� Ÿ���� ���� ������, ��� �� �ڿ��� ��Ÿ�� ���� �����Ѵ�.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", "100 %\n") +
                                           string.Format(explainfmt2, "����", "���� ü���� ���� ���� �� �Ѹ�\n") +
                                           string.Format(explainfmt2, "�����", "�⺻ ���ݷ� + " + a_MutantCtrl.GetSk1Dmg(StarF) + "\n") +
                                           string.Format(explainfmt2, "��Ÿ��", "���� ���� �� 1ȸ �ߵ�");
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "����";
                        explainText.text = string.Format(explainfmt1, "�������̷� ���� ������ Ưȭ�� ��ü�� �����.", "������ ���ϴ� ���ؿ� �ڽ��� �޴� ���ذ� �����Ѵ�.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", "�нú�\n") +
                                           string.Format(explainfmt2, "����", "�ڱ��ڽ�\n") +
                                           string.Format(explainfmt2, "�ִ� ����� ������", a_MutantCtrl.MutSk2AMul * 100 + "%\n") +
                                           string.Format(explainfmt2, "�޴� ����� ������", a_MutantCtrl.MutSk2TMul * 100 + "%\n") +
                                           string.Format(explainfmt2, "��Ÿ��", "����");
                        break;
                }
                break;

            case MonsterName.Jammo:
                Jammo a_JammoCtrl = MonInformMgr.Instance.GetComponentInChildren<Jammo>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>�鸸��Ʈ</color>";
                        explainText.text = string.Format(explainfmt1, "������ �����κ��� ������ ���⸦ ������ ���� �����Ѵ�", "���� ������ ū ���ظ� �ش�.") +
                                           string.Format(explainfmt2, "����", "���� ���� ���� ���\n") +
                                           string.Format(explainfmt2, "�����", a_JammoCtrl.GetUltiDmg(StarF));
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "����";
                        explainText.text = string.Format(explainfmt1, "ü���� ȸ�θ� ���������� Ȱ��ȭ�� ������ ���°� �ȴ�.", "���� �⺻������ ��ȭ�ȴ�.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", a_JammoCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n") +
                                           string.Format(explainfmt2, "����", "���� ���� ���� ���\n") +
                                           string.Format(explainfmt2, "��ȭ�� �����", "�⺻ ���ݷ��� " + a_JammoCtrl.GetSk1Mul(StarF) + "��\n") +
                                           string.Format(explainfmt2, "��Ÿ��", a_JammoCtrl.SkSet.Cool[(int)Skill.Sk1] + "��");
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "����";
                        explainText.text = string.Format(explainfmt1, "�̾��� ������ ������� �������� ����Ų��.", "���� �Ʊ��� ���� �ӵ��� ���ȴ�.") +
                                           string.Format(explainfmt2, "�ߵ� Ȯ��", a_JammoCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n") +
                                           string.Format(explainfmt2, "���ӽð�", a_JammoCtrl.JamSk2Time + "��\n") +
                                           string.Format(explainfmt2, "����", "�ڽ����κ��� " + a_JammoCtrl.JamSk2Radius + " �Ÿ�\n") +
                                           string.Format(explainfmt2, "������", a_JammoCtrl.GetSk2Value(StarF) + "\n") +
                                           string.Format(explainfmt2, "��Ÿ��", a_JammoCtrl.SkSet.Cool[(int)Skill.Sk2] + "��");
                        break;
                }
                break;
        }
    }
}
