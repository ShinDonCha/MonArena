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
                        explainText.text = "��ǥ�� �����Ͽ� �ٹ̻����� �����Ѵ�.\n�ٹ̻����� ���� �ε��� �����Ͽ� �ֺ��� �ִ� ���鿡�� ���ظ� �ش�.\n\n" +
                                            "<color=#ff0000>���� :</color> ���߷κ��� " + a_SoldierCtrl.SolUltiRadius + " �Ÿ�\n" +
                                            "<color=#ff0000>����� :</color> " + a_SoldierCtrl.GetUltiDmg(StarF);
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "�๰ ����";
                        explainText.text = "�ڽſ��� �๰�� �����Ͽ� ������縦 Ȱ��ȭ�Ѵ�.\n�ñر� �������� �����ǰ� �� �ݵ����� �������� HP�� �����Ѵ�.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> " + a_SoldierCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n" +
                                            "<color=#ff0000>������ ������ :</color> " + a_SoldierCtrl.SolSk1UltiG + "\n" +
                                            "<color=#ff0000>�Ҹ� HP :</color> ���� ü���� " + a_SoldierCtrl.GetDecRatio(StarF) * 100 + "%\n" +
                                            "<color=#ff0000>���� :</color> �ڱ� �ڽ�\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> " + a_SoldierCtrl.SkSet.Cool[(int)Skill.Sk1] + "��";
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "������û";
                        explainText.text = "��������� ���� ��� 4�븦 ȣ���Ѵ�.\n����� �������� �̵��ϸ� ���� �߰��ϸ� ������ ���Ѵ�.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> " + a_SoldierCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n" +
                                            "<color=#ff0000>���� :</color> �������� �ִ� ������ ����\n" +
                                            "<color=#ff0000>��� ���ӽð� :</color> " + a_SoldierCtrl.DronLifeT + "��\n" +
                                            "<color=#ff0000>��� ���ݼӵ� :</color> �ʴ� " + 1 / a_SoldierCtrl.DronASpd + "��\n" +
                                            "<color=#ff0000>��� ����� :</color> �ߴ� " + a_SoldierCtrl.GetDronDmg(StarF) + "\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> " + a_SoldierCtrl.SkSet.Cool[(int)Skill.Sk2] + "��";
                        break;
                }
                break;

            case MonsterName.Zombie:
                Zombie a_ZombieCtrl = MonInformMgr.Instance.GetComponentInChildren<Zombie>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>������ ����</color>";
                        explainText.text = "�ǿ� ���� ������ �������� �����κ��� ������� ���Ѵ´�.\n���� ���� ���鿡�� ���ظ� �ְ� ���ط��� �Ϻθ� �ڽ��� ü������ ��ȯ�Ѵ�.\n\n" +
                                            "<color=#ff0000>���ӽð� :</color> " + a_ZombieCtrl.ZomUltiTime + "��\n" +
                                            "<color=#ff0000>���� :</color> �ڽ����κ��� " +  a_ZombieCtrl.ZomUltiRadius + " �Ÿ�\n" +
                                            "<color=#ff0000>ü����ȯ�� :</color> " + a_ZombieCtrl.ZomUltiRatio * 100 + "%\n" +
                                            "<color=#ff0000>����� :</color> �ʴ� " + a_ZombieCtrl.GetUltiDmg(StarF);
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "����";
                        explainText.text = "���� ���� ������Ų��.\n������ ���� ���ظ� �԰� ���� �ð� ��ȭ�ȴ�.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> " + a_ZombieCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n" +
                                            "<color=#ff0000>���ӽð� :</color> " + a_ZombieCtrl.ZomSk1SusTime + "��\n" +
                                            "<color=#ff0000>���� :</color> ���� ���� ���� ���\n" +
                                            "<color=#ff0000>��/���� ���ҷ� :</color> " + a_ZombieCtrl.GetSk1ReduVal(StarF) + "\n" +
                                            "<color=#ff0000>����� :</color> " + a_ZombieCtrl.GetSk1Dmg(StarF) + "\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> " + a_ZombieCtrl.SkSet.Cool[(int)Skill.Sk1] + "��";
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "��ȿ";
                        explainText.text = "������ ������ ������ ���Ǹ� ����.\n���� �Ÿ� �ȿ� �ִ� ���� ������ ���ݷ��� ���ҽ�Ű�� �ڽ��� �����ϰ� �Ѵ�.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> " + a_ZombieCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n" +
                                            "<color=#ff0000>���ӽð� :</color> " + a_ZombieCtrl.ZomSk2SusTime + "��\n" +
                                            "<color=#ff0000>���� :</color> �ڽ����κ��� " + a_ZombieCtrl.ZomSk2Radius + " �Ÿ�\n" +
                                            "<color=#ff0000>���ݷ� ������ :</color> " + a_ZombieCtrl.GetReduceRatio(StarF) * 100 + "%\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> " + a_ZombieCtrl.SkSet.Cool[(int)Skill.Sk2] + "��";
                        break;
                }
                break;

            case MonsterName.GamblerCat:
                GamblerCat a_GamblerCatCtrl = MonInformMgr.Instance.GetComponentInChildren<GamblerCat>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>���ȸ��</color>";
                        explainText.text = "���⿡ ó�� �Ʊ��� ���س���.\n���� ���� ���� �� �Ʊ��� ü���� ũ�� ȸ����Ų��.\n\n" +
                                            "<color=#ff0000>���� :</color> �ڽ����κ��� " + a_GamblerCatCtrl.CatUltiRadius + " �Ÿ�\n" +
                                            "<color=#ff0000>ȸ���� :</color> " + a_GamblerCatCtrl.GetUltiAmount(StarF);
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "ġ��";
                        explainText.text = "������ ����� �����Ͽ� �Ʊ��� ���´�.\n�Ʊ� �� ���� ü���� ȸ����Ų��.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> " + a_GamblerCatCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n" +
                                            "<color=#ff0000>���� :</color> ���� ü���� ���� ���� �Ʊ� �� ��\n" +
                                            "<color=#ff0000>ȸ���� :</color> " + a_GamblerCatCtrl.GetHealAmount(StarF) + "\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> " + a_GamblerCatCtrl.SkSet.Cool[(int)Skill.Sk1] + "��";
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "�޺���";
                        explainText.text = "������ ���ؾ��� �Ʊ��� ��⸦ �ø���.\n�Ʊ� �� ���� ������� ������Ų��.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> " + a_GamblerCatCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n" +
                                            "<color=#ff0000>���ӽð� :</color> " + a_GamblerCatCtrl.GetBuffTime(StarF) + "��\n" +
                                            "<color=#ff0000>���� :</color> ���� ���ݷ��� ���� ���� �Ʊ� �� ��\n" +
                                            "<color=#ff0000>����� ������ :</color> " + a_GamblerCatCtrl.CatSk2IncAmount * 100 + "%\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> " + a_GamblerCatCtrl.SkSet.Cool[(int)Skill.Sk2] + "��";
                        break;
                }
                break;

            case MonsterName.Mutant:
                Mutant a_MutantCtrl = MonInformMgr.Instance.GetComponentInChildren<Mutant>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>������</color>";
                        explainText.text = "�Ҹ����� ���� �ӵ��� �̵��ϸ� ��ī�ο� �������� ������ �������Ѵ�.\n���� ���� ���� ���鿡�� ���ظ� �ش�.\n\n" +
                                            "<color=#ff0000>���� :</color> �ڽ��� ����������� �ִ� " + a_MutantCtrl.MutUltiRange + " �Ÿ����� ��(������ �浹 �� ���� ����)\n" +
                                            "<color=#ff0000>����� :</color> " + a_MutantCtrl.GetUltiDmg(StarF);
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "�ϻ�";
                        explainText.text = "���� ȯ�濡 ������ ���� ���� �� ���� �޼Ҹ� �����Ѵ�.\n���� ���� �� ���� ���� ���� Ÿ���� ���� ������, ��� �� �ڿ��� ��Ÿ�� ���� �����Ѵ�.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> 100%\n" +
                                            "<color=#ff0000>���� :</color> ���� ü���� ���� ���� �� �Ѹ�\n" +
                                            "<color=#ff0000>����� :</color> �⺻ ���ݷ� + " + a_MutantCtrl.GetSk1Dmg(StarF) + "\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> ���� ���� �� 1ȸ �ߵ�";
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "����";
                        explainText.text = "�������̷� ���� ������ Ưȭ�� ��ü�� �����.\n������ ���ϴ� ���ؿ� �ڽ��� �޴� ���ذ� �����Ѵ�.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> �нú�\n" +
                                            "<color=#ff0000>���� :</color> �ڱ� �ڽ�\n" +
                                            "<color=#ff0000>�ִ� ����� ������ :</color> " + a_MutantCtrl.MutSk2AMul * 100 + "%\n" +
                                            "<color=#ff0000>�޴� ����� ������ :</color> " + a_MutantCtrl.MutSk2TMul * 100 + "%\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> ����";
                        break;
                }
                break;

            case MonsterName.Jammo:
                Jammo a_JammoCtrl = MonInformMgr.Instance.GetComponentInChildren<Jammo>();   //MonInformMgr������Ʈ�� ������ �ִ� ���� ������Ʈ�κ��� ��ũ��Ʈ ��������

                switch (CurExList)     //SkillGroup���� ���õ� ��ų�� ���� ����� ���� ����
                {
                    case ExplainList.Ultimate:
                        nameText.text = "<color=#ffff00>�鸸��Ʈ</color>";
                        explainText.text = "������ �����κ��� ������ ���⸦ ������ ���� �����Ѵ�.\n���� ������ ū ���ظ� �ش�.\n\n" +
                                            "<color=#ff0000>���� :</color> ���� ���� ���� ���\n" +
                                            "<color=#ff0000>����� :</color> " + a_JammoCtrl.GetUltiDmg(StarF);
                        break;

                    case ExplainList.Skill1:
                        nameText.text = "����";
                        explainText.text = "ü���� ȸ�θ� ���������� Ȱ��ȭ�� ������ ���°� �ȴ�.\n���� �⺻������ ��ȭ�ȴ�.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> " + a_JammoCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n" +
                                            "<color=#ff0000>���� :</color> ���� ���� ���� ���\n" +
                                            "<color=#ff0000>��ȭ�� ����� :</color> �⺻ ���ݷ��� " + a_JammoCtrl.GetSk1Mul(StarF) + "��\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> " + a_JammoCtrl.SkSet.Cool[(int)Skill.Sk1] + "��";
                        break;

                    case ExplainList.Skill2:
                        nameText.text = "����";
                        explainText.text = "�̾��� ������ ������� �������� ����Ų��.\n���� �Ʊ��� ���� �ӵ��� ���ȴ�.\n\n" +
                                            "<color=#ff0000>�ߵ� Ȯ�� :</color> " + a_JammoCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n" +
                                            "<color=#ff0000>���ӽð� :</color> " + a_JammoCtrl.JamSk2Time + "��\n" +
                                            "<color=#ff0000>���� :</color> �ڽ����κ��� " + a_JammoCtrl.JamSk2Radius + " �Ÿ�\n" +
                                            "<color=#ff0000>������ :</color> " + a_JammoCtrl.GetSk2Value(StarF) + "\n" +
                                            "<color=#ff0000>��Ÿ�� :</color> " + a_JammoCtrl.SkSet.Cool[(int)Skill.Sk2] + "��";
                        break;
                }
                break;
        }
    }
}
