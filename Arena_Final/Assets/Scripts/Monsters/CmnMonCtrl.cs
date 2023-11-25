using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

//���͵��� �������̳� ��ų��� ���� �� ���� ������ ��� ��ȭ�ϴ� ������ ��Ʈ���ϴ� ��ũ��Ʈ
//�ʱ� �� ���� �ܿ� �ܺο��� ������ ������ �� ��� ���⸦ ����
//�� ���͵��� ���� ��ũ��Ʈ(Zombie,Soldier ��)�� ��ӵȴ�.
public abstract class CmnMonCtrl : CmnMonSet, IPunObservable
{    
    protected abstract void SkillSetting();       //������ ��ų Ȯ��&��Ÿ�� ���� �Լ�(���� ���� �����ϹǷ� ���� ��ũ��Ʈ���� ����)    
    private bool attackEnable = true;             //���� �������� �Ǵ��ϴ� ���� (���� ������ ���� false����)
    private readonly Queue<int> actQueue = new(); //�ߵ��� ������ ��Ƽ�� ��ų ���
    public GameObject Enemy { get; set; } = null; //���� Ÿ��

    #region ------------------ PhotonSerializeView ------------------
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)      //���ð� ������ ��������
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CMCcurStat.hp);
            stream.SendNext(CMCcurStat.attackDmg);
            stream.SendNext(CMCcurStat.defPower);
            stream.SendNext(CMCcurStat.mdefPower);
            stream.SendNext(CMCcurStat.attackSpd);
            stream.SendNext(CMCcurStat.moveSpd);
            stream.SendNext(curAddStat.MulAtkDmg);
            stream.SendNext(curAddStat.MulTakeDmg);
            stream.SendNext(curAddStat.AddAtkDmg);
            stream.SendNext(curAddStat.DecDefence);
            stream.SendNext(curAddStat.DecMDefence);
            stream.SendNext(hpImg.fillAmount);
        }
        else
        {                   
            CMCcurStat.hp = (int)stream.ReceiveNext();
            CMCcurStat.attackDmg = (int)stream.ReceiveNext();
            CMCcurStat.defPower = (int)stream.ReceiveNext();
            CMCcurStat.mdefPower = (int)stream.ReceiveNext();
            CMCcurStat.attackSpd = (float)stream.ReceiveNext();
            CMCcurStat.moveSpd = (float)stream.ReceiveNext();
            curAddStat.MulAtkDmg = (float)stream.ReceiveNext();
            curAddStat.MulTakeDmg = (float)stream.ReceiveNext();
            curAddStat.AddAtkDmg = (int)stream.ReceiveNext();
            curAddStat.DecDefence = (int)stream.ReceiveNext();
            curAddStat.DecMDefence = (int)stream.ReceiveNext();
            hpImg.fillAmount = (float)stream.ReceiveNext();
        }
    }
    #endregion ------------------ PhotonSerializeView ------------------

    protected override void Start()
    {
        GameManager.StartEvent += StartFight;       //���� ���� �� ������ �Լ� ��Ͽ� �߰�
        base.Start();
    }

    //Update is called once per frame
    void Update()
    {
        if (photonV != null && !photonV.IsMine)     //��Ƽ �����϶� IsMine�� �ƴ� ��ü�� �������� ����
            return;

        if (attackRangeCol.enabled && attackRangeCol.radius < 15.0f)         //���� ��� ���� �ݶ��̴��� Enable�����̰� ������ ũ�⺸�� ������
            attackRangeCol.radius += 0.1f;          //SphereCollider ũ�� ���� �������Ѽ� �� ���� ���� Ȯ��

        #region -------------------------- ���� ���� üũ, ���� --------------------------
        switch (monState)
        {
            case MonsterState.Search:   //���� ����� ã�� ����
                attackRangeCol.enabled = true;   //���� ��ǥ Ž���� ���� SphereCollider Ȱ��ȭ
                break;

            case MonsterState.Trace:    //������ ���ݴ���� �Ѿư��� ����
                if (Enemy.activeSelf)       //���ݴ���� ����ִٸ�
                {
                    if ((Enemy.transform.position - transform.position).magnitude <= CMCcurStat.attackRange)     //���� ��Ÿ� �ȿ� ���� �ְ�
                        if (attackEnable)    //���� ������ ���¸�
                        {
                            AnimReq(MonsterState.Attack.ToString());   //���ݻ��·� ����
                            navAgent.avoidancePriority = atkPriority;  //�׺��� ȸ�ǿ켱���� ����(�ٸ� ���Ͱ� �� ���͸� ���ذ����� �ϱ�����)
                        }
                        else//(attackEnable) //���� �Ұ����� ���¸�
                        {
                            AnimReq(MonsterState.Rest.ToString());     //�޽� ���·� ����
                        }
                }
                else //(!Enemy.activeSelf) //���ݴ���� �������¸�
                    AnimReq(MonsterState.Search.ToString());    //Ž�����·� ����
                break;

            case MonsterState.Rest:     //�Ϲݰ����� ������ �� ��� ���� ����
                if (Enemy.activeSelf)        //���ݴ���� ����ִٸ�
                {
                    if ((Enemy.transform.position - transform.position).magnitude <= CMCcurStat.attackRange)     //���� ��Ÿ� �ȿ� ���� �ְ�
                    {
                        if (attackEnable)   //���� ������ ���¸�
                            AnimReq(MonsterState.Attack.ToString());    //���ݻ��·� ����
                    }
                    else//((Enemy.transform.position - transform.position).magnitude > CMCcurStat.attackRange)   //���� ���� ��Ÿ� �ۿ� �ִٸ�
                    {
                        AnimReq(MonsterState.Trace.ToString());         //�������·� ����
                        navAgent.avoidancePriority = movePriority;      //�׺��� ȸ�ǿ켱���� ����
                    }
                }
                else //(!Enemy.activeSelf)  //���� ����� ���� ���¸�
                {
                    navAgent.avoidancePriority = movePriority;  //�׺��� ȸ�ǿ켱���� ����
                    AnimReq(MonsterState.Search.ToString());    //Ž�����·� ����
                }
                break;

            case MonsterState.Die:     //��������
                if (photonV == null)   //�̱۰����� ��
                {
                    ActiveSet(false);   //�� ���͸� ���忡�� ������� �ϱ� 
                    DeathEffInst();     //��� ����Ʈ ����
                    GameMgr.Instance.CalcResults(tag);   //��� ����� ���� �� ������ �±� ����
                }
                else    //��Ƽ������ ��
                {
                    photonV.RPC("ActiveSet", RpcTarget.All, false); //�� ���͸� ���忡�� ������� �ϱ�
                    photonV.RPC("DeathEffInst", RpcTarget.All);     //��� ����Ʈ ����
                    MtGameMgr.Instance.CalcResults(tag);            //��� ����� ���� �� ������ �±� ����
                }
                break;
        }
        #endregion -------------------------- ���� ���� üũ, ���� --------------------------
    }

    [PunRPC]
    protected void DeathEffInst()     //��� ����Ʈ ������ ���� RPC
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
    }

    [PunRPC]
    protected void ActiveSet(bool OnOff)    //���ӿ�����Ʈ�� SetActive ������ ���� RPC
    {
        gameObject.SetActive(OnOff);
    }

    private void FixedUpdate()
    {
        if (photonV != null && !photonV.IsMine)     //��Ƽ �����϶� IsMine�� �ƴ� ��ü�� �������� ����
            return;

        #region -------------------------- ���� �������� --------------------------
        if (Enemy == null || !Enemy.activeSelf)      //���� ����� ������� ���
            return;

        switch (monState)
        {
            case MonsterState.Trace:            
                transform.rotation = Quaternion.LookRotation(Enemy.transform.position - transform.position);    //���� ����� ���� ȸ��
                break;

            case MonsterState.Attack: //���ݻ�Ÿ� �ȿ� ���� ����� ������ ����� (���� ��� Update()���� Trace�� ���°� ����ȴ�.)
                if (attackEnable)     //���� ���� ������ ����
                {    
                    Quaternion a_EnemyQ = Quaternion.LookRotation(Enemy.transform.position - transform.position);   //�� ���Ͱ� ����� �ٶ󺼶��� ȸ����(���ʹϾ�)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, a_EnemyQ, rotAngle);          //�� ���͸� ȸ�� (fixedTime�� �ִ� rotAngle��ŭ ȸ��)
                }                
                break;
        }
        #endregion -------------------------- ���� �������� --------------------------
    }

    private void OnTriggerEnter(Collider other)   //�ݶ��̴� �浹 üũ
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)) //������ ������ �ִ� MonsterSkin�� Layer�� MonBody
        {
            if (CompareTag(other.transform.parent.tag))  //�Ʊ��̶�� ���
                return;

            EnemySet(other.transform.parent.gameObject);//�浹�� ����� ���� ��ǥ�� ����
            AnimReq(MonsterState.Trace.ToString());     //���� �ִϸ��̼� ���
            attackRangeCol.enabled = false;             //���� ��ǥ ������ ���Ǵ� SphereCollider ��Ȱ��ȭ
        }
    }

    protected virtual void StartFight()      //���� ���� �� ����Ǵ� �Լ�(Mutant�� ������ StartFight�� ��� ��)
    {
        GetComponentInChildren<StCanvasCtrl>(true).gameObject.SetActive(true);     //HP�� ǥ�ø� ���� ĵ���� On���·� ����

        if (photonV != null && !photonV.IsMine)     //��Ƽ ������ �������� ����
            return;

        AnimReq(MonsterState.Search.ToString());    //Ž�� �ִϸ��̼� ����
    }

    private void AnimReq(string ParamName)    //�ִϸ��̼� �����û �Լ� (������ ���� ����üũ�� ���� monState ������ �� �ִϸ��̼��� ���ۺκп��� �����ȴ�.)
    {
        if (photonV == null)  //�̱۰����� ��
        {
            AnimSet(ParamName);
        }
        else//(photonV != null)     //��Ƽ������ �� (AnimReq�Լ��� IsMine������ ����ȴ�.)
        {
            photonV.RPC("AnimSet", RpcTarget.All, ParamName);
        }
    }

    [PunRPC]
    protected void AnimSet(string ParamName) //�ִϸ��̼� ��� �Լ�(�̱�, ��Ƽ ����)
    {
        animator.SetTrigger(ParamName);     //�ش� �ִϸ��̼� ���
        monState = MonsterState.Wait;       //�ִϸ��̼� ��� ������ �����·� ����
    }

    public void StateSet(string StateName)   //�ִϸ��̼� ��� �� ������� �ִϸ��̼ǿ� ���� ������ ���¸� �ٲ��ֱ� ���� �Լ� (�ִϸ��̼��� ���ۺκп��� �̺�Ʈ�� ����)
    {
        if (!Enum.TryParse(StateName, out MonsterState MS))
            return;

        monState = MS;      //���� ���� ����

        if (photonV != null && !photonV.IsMine)     //��Ƽ ������ �������� ����
            return;

        if (MS.Equals(MonsterState.Trace))      //���� ���·� ����� �����̶��
        {
            navAgent.isStopped = false;         //�׺� �������·� ����
            navAgent.destination = Enemy.transform.position;    //�� ������ ������ ����
        }
        else//(!MS.Equals(MonsterState.Trace))  //�� �� ���·� ����� �����̶��
            navAgent.isStopped = true;          //�׺� Ȱ�����·� ����
    }

    public void EnemySet(GameObject GO)     //���� ��ǥ ���� �Լ�
    {
        if (photonV == null)        //�̱� �÷���
        {
            Enemy = GO;             //�Ű������� ���� ���� ������Ʈ�� ���ݸ�ǥ�� ����
        }
        else if (photonV != null && photonV.IsMine)     //��Ƽ �÷���(IsMine�� ����)
        {
            if (Enum.TryParse(GO.name[..GO.name.IndexOf("(")], out MonsterName EnemyMN))      //���ݴ������ �������� ��ǥ�� �̸��� MonsterName���� ��ȯ
                photonV.RPC("MultiEnemySet", RpcTarget.All, EnemyMN);      //���ð� ���ݿ��� ���� ����� ������ �ϱ����� RPC ����
        }
    }

    [PunRPC]
    protected void MultiEnemySet(MonsterName EnemyName)  //��Ƽ�� ���� ��ǥ ���� �Լ�
    {
        if (!Enum.TryParse(tag, out Team MyTeamTag))     //�� ������ �±׸� Enum�������� ����
            return;

        switch (MyTeamTag)
        {
            case Team.Team1:
                Enemy = FindClass.GetMonCMC(Team.Team2.ToString(), EnemyName).gameObject;      //���ݴ�� ����
                break;

            case Team.Team2:
                Enemy = FindClass.GetMonCMC(Team.Team1.ToString(), EnemyName).gameObject;      //���ݴ�� ����
                break;
        }
    }    

    #region ----------------------- ����, ��Ƽ�� ��ų -----------------------
    protected virtual void Attack()         //�Ϲ� ���� �� ���� (�Ϲ� ���� �ִϸ��̼� Ư���κп��� �̺�Ʈ�� ����)
    {
        if (photonV != null && !photonV.IsMine)   //��Ƽ �����϶� IsMine�� �ƴ� ��ü�� �������� ����
            return;

        StartCoroutine(AttackDelayCalc());  //���� ������ ���
        ActSkillCheck();                    //��Ƽ�� ��ų�ߵ� üũ
        AddUltiGage(15);                    //�����Ҷ� ���� �ñر� ������ 15 ȹ��

        if (UltiGage == 100)                //������ �� ä���� �� �ñ� ��ų �ߵ�
            AnimReq(Skill.Ultimate.ToString());   //�ñر� �ִϸ��̼� ��� ��û

        int a_ActSkCount = actQueue.Count;  //�ߵ��� ����� ��Ƽ�� ��ų�� �� ��������

        for (int i = 0; i < a_ActSkCount; i++)    //�ߵ��� ����� ��Ƽ�� ��ų(Skill1, Skill2)�� ���� ���
            AnimReq(((Skill)actQueue.Dequeue()).ToString());     //��ų��ȣ�� �°� �ִϸ��̼� ���
    }

    private IEnumerator AttackDelayCalc()      //�Ϲ� ���ݿ� �����̸� �����Ű�� ���� �ڷ�ƾ
    {
        attackEnable = false;       //�Ϲݰ��� �Ұ��� ���·� ����
        yield return new WaitForSeconds(CMCcurStat.attackSpd);   //���ݼӵ���ŭ ���� �ڿ� ����� ������
        attackEnable = true;        //�Ϲݰ��� ���� ���·� ����
    }

    private void ActSkillCheck()            //��Ƽ�� ��ų �ߵ� üũ �Լ�(�� ��ų���� �ߵ� Ȯ���� ����������)
    {
        for (int i = 0; i < (int)Skill.Count; i++)   //�ñر⸦ ������ ��ų ������ŭ ���� (��ų���� ���� �ѹ��� �ߵ� Ȯ���� ���Ǳ� ������ �������� ���� �ߵ���ų���� ����)
        {
            int a_Rnd = UnityEngine.Random.Range(0, 100);

            if (skillOnOff[i])       //�ش� ��ų�� ��� ���ɻ��¸� (��ų��Ÿ���� �ƴ� �� ���ɻ���)
            {
                if (actQueue.Contains(i))      //�̹� ���� ������ ��Ƽ�� ��ų�� ��ϵǾ� ���� ��� �Ѿ��
                    continue;

                if (a_Rnd < SkSet.Prob[i])     //������ ��ų �ߵ�Ȯ���� �����ϸ� ť�� �߰�
                    actQueue.Enqueue(i);
            }
            else//(!skillOnOff[i])  //�ش� ��ų�� ��� �Ұ��ɻ��¸� �Ѿ��(��ų��Ÿ���� �ƴ� �� ���ɻ���)
                continue;
        }
    }    

    private IEnumerator ActSkillOn(int SkillNum)        //��ų �ߵ� (Skill1�� Skill2 �ִϸ��̼� Ư���κп��� �̺�Ʈ�� ����)
    {
        switch ((Skill)SkillNum)
        {
            case Skill.Sk1:
                Skill1();   //1�� ��ų �ߵ�
                break;

            case Skill.Sk2:
                Skill2();   //2�� ��ų �ߵ�
                break;
        }

        skillOnOff[SkillNum] = false;  //�ش� ��ų ��� �Ұ��� ���·� ����
        yield return new WaitForSeconds(SkSet.Cool[SkillNum]);     //������� �����޾��� �� SkillNum�� �ڷ�ƾ ���۽� �Ѱܹ��� �Ű������� �°� ���� ����, �� �ڷ�ƾ���� �ٸ� �ѹ� ����
        skillOnOff[SkillNum] = true;   //�ش� ��ų ��� ���ɻ��·� ����
    }

    protected virtual void Skill1()     //��ų 1 �ߵ� �� ���Ǵ� ��ũ��Ʈ(���� �� ���� ��ũ��Ʈ���� ����)
    {
    }

    protected virtual void Skill2()     //��ų 2 �ߵ� �� ���Ǵ� ��ũ��Ʈ(���� �� ���� ��ũ��Ʈ���� ����)
    {
    }
    #endregion ----------------------- ����, ��Ƽ�� ��ų -----------------------

    #region ------------------- �ñر� ���� -------------------
    private void TimePause()        //�ñر� ��� �� ȿ���� �ֱ� ���� ���� (�ñر� �ִϸ��̼� ������������ �̺�Ʈ�� ����)
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.UltiReady);     //�ñر� ���� ȿ���� ���
        Instantiate(ultiEffect, transform);       //�ñر� ����Ʈ ����
        Time.timeScale = 0.0f;      //�Ͻ����� ���·� ����
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;   //�� ������ �ִϸ��̼Ǹ� ����ǵ��� ��� ����
        animator.speed = 0.6f;      //�ִϸ��̼� �ӵ� ����
    }

    protected virtual void UltiSkill()    //�ñر� �ߵ� �Լ�(�ñر� �ִϸ��̼ǿ��� �̺�Ʈ�� ����)
    {        
        UltiGage = 0;                 //�ñر� ������ �ʱ�ȭ
        Time.timeScale = 1.0f;        //�Ͻ����� ����
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;   //���� �ִϸ��̼� ���� ����
        animator.speed = 1.0f;        //�ִϸ��̼� �ӵ� �ʱ�ȭ
    }

    protected void AddUltiGage(int UG)     //�ñر� ������ ���� �Լ�
    {
        UltiGage += UG;        //������ �߰�

        if (UltiGage > 100)     //�ִ��ġ�� 100�� �ѱ��� ���ϵ���
            UltiGage = 100;
    }
    #endregion ------------------- �ñر� ���� -------------------

    #region -------------------- ����� ������ --------------------
    public int GiveDamage(GameObject Target, int Damage)     //���� �� ��� ��ũ��Ʈ�� �����ؼ� ����� �����Ű�� �Լ� (�Ϲݰ���, ��ų���� ���� �� ��ũ��Ʈ�� ����� ����)
    {
        if (!Target.activeSelf)     //���� �׾��ٸ� ���
            return 0;

        Damage = (int)((Damage + curAddStat.AddAtkDmg) * (1.0f + curAddStat.MulAtkDmg));         //���� �����ڰ� ��� �ִ� ����� ������ ��ŭ �� ����� ����
        curAddStat.AddAtkDmg = 0;     //�߰������ �ʱ�ȭ

        int a_CalcDmg = Target.GetComponent<CmnMonCtrl>().TakeDamage(CMCcurStat.attackType, Damage);       //������ ���� ����� ���, ����
        TotalDmg += a_CalcDmg;        //������ ���� ����� ����

        return a_CalcDmg;
    }

    private int TakeDamage(AttackType AType, int Damage)       //���� �޾��� �� ����Ǵ� �Լ�(�������� ����Ÿ��, �����)
    {
        if (photonV != null && !photonV.IsMine)
            return 0;

        if (CMCcurStat.hp > 0)          //�� ���Ͱ� �����������..
        {
            Damage = (int)(Damage * (1.0f + curAddStat.MulTakeDmg));        //�޴� ���ط� ������ ��ŭ ���ط� ����

            switch (AType)              //����� ���
            {
                case AttackType.Physical:               //���� ������ ���� ������ ���
                    Damage -= CMCcurStat.defPower;      //���� ���¸�ŭ ����
                    break;

                case AttackType.Magical:                //���� ������ ���� ������ ���
                    Damage -= CMCcurStat.mdefPower;     //���� �������׷� ��ŭ ����
                    break;
            }

            Damage = Mathf.Max(0, Damage);      //������� ���� ������ �ʰ� �ϱ� ����

            switch (CMCcurStat.hp > Damage)       //���� ü�°� ���� ����� ��
            {
                case true: //(CMCcurStat.hp > Damage)
                    if (photonV != null && !photonV.IsMine)     //���ݹ޴� �ְ� ��Ƽ �����̸� ���� ������� �������� ����� ������� ����(������ �����Դ°� ���ÿ��� ����ǵ��� �ϱ� ����)
                        return Damage;
                    break;

                case false: //(CMCcurStat.hp <= Damage)
                    Damage = CMCcurStat.hp;

                    if (photonV != null && !photonV.IsMine)     //���ݹ޴� �ְ� ��Ƽ �����̸� ���� ������� �������� ����� ������� ����(������ �����Դ°� ���ÿ��� ����ǵ��� �ϱ� ����)
                        return Damage;

                    monState = MonsterState.Die;    //���� ���ó��
                    break;
            }

            //-------------- ���� ���� ��� (���ݹ޴� �ְ� �̱۰� ��ƼIsMine�� ���� ������� ����)
            CMCcurStat.hp -= Damage; //����� ������
            AddUltiGage(10);         //���� �� ���� �ñر� ������ 10�� ����
            hpImg.fillAmount = (float)CMCcurStat.hp / CMCmonStat.hp;    //���� �Ӹ����� HP�� ǥ�� ����
            TextRequest.InstantTxtReqAct(transform.position, TxtAnimList.DmgTxtAnim, Damage);       //����� �ؽ�Ʈ ��� ��û
            TotalHP += Damage;  //���� �� ���ؿ� �߰�
            return Damage;      //���� ���ظ� �����ڿ��� ���� (�������� ����� ���տ� �����ֱ� ����)
            //-------------- ���� ���� ��� (���ݹ޴� �ְ� �̱۰� ��ƼIsMine�� ���� ������� ����)
        }
        else
        {
            if (!monState.Equals(MonsterState.Die))     //Ȥ�� ������ �������·� üũ�Ǿ����� �ʴٸ� ����
                monState = MonsterState.Die;

            return 0;     //�� ���Ͱ� �������¸� ����� ����
        }
    }
    #endregion -------------------- ����� ������ --------------------    

    #region --------------------------- ���� ��Ƽ�� ��ų ---------------------------
    public void TakeAny(TakeAct TA, float Figure, float SusTime = 0.0f)     //���� ��Ƽ�� ��ų (����, ��ġ, ���ӽð�) �ܺο��� ����� ����
    {
        if (photonV != null && !photonV.IsMine)     //��Ƽ �����϶� IsMine�� �ƴ� ��ü�� �������� ����
            return;

        switch (TA)
        {
            case TakeAct.Heal:      //������ ���� ���
                TakeHeal((int)Figure);
                break;

            default:                //�� ���� ���
                StartCoroutine(BuffTextCo(Figure, SusTime, TA));
                break;            
        }
    }

    private void TakeHeal(int Amount)     //���� ȸ�� ��� �Լ�
    {
        if ((CMCcurStat.hp + Amount) > CMCmonStat.hp)  //ȸ������ �ִ�ü���� �ѰԵǸ� �ִ�ü�±����� �ݿ�
            Amount = CMCmonStat.hp - CMCcurStat.hp;    //���� ȸ���� ���

        CMCcurStat.hp += Amount;         //���� ü�� ȸ��
        TextRequest.InstantTxtReqAct(transform.position, TxtAnimList.HealTxtAnim, Amount);       //�� �ؽ�Ʈ ��� ��û
        hpImg.fillAmount = (float)CMCcurStat.hp / CMCmonStat.hp;    //���� �Ӹ����� HP�� ǥ�� ����
    }

    private IEnumerator BuffTextCo(float Figure, float SusTime, TakeAct Act) //����or���� ��ġ, ���ӽð�, ���º�ȭ ����
    {
        //--------- ���� ��ġ�� ���� �������� ��������� ����
        TxtAnimList a_TxtAList;

        if (Figure > 0)
            a_TxtAList = TxtAnimList.BuffTxtAnim;
        else if (Figure < 0)
            a_TxtAList = TxtAnimList.DeBuffTxtAnim;
        else//(Figure = 0)
            yield break;
        //--------- ���� ��ġ�� ���� �������� ��������� ����

        if(photonV == null)         //�̱�
            TextRequest.BuffTxtReqAct(tag, transform.parent.GetSiblingIndex(), transform.position, a_TxtAList, Act);     //���º�ȭ �ؽ�Ʈ ���
        else//(photonV != null)     //��Ƽ(��Ƽ�� TakeAny�� IsMine�� �����)
            TextRequest.BuffTxtReqAct(tag, (int)CMCmonStat.monName, transform.position, a_TxtAList, Act);   //���º�ȭ �ؽ�Ʈ ���(������ 5�����̶� �����ѵ� ���� �þ�� �Ұ�����)

        switch (Act)
        {
            case TakeAct.Dmg:
                curAddStat.MulAtkDmg += Figure;             //����� ������ ���ϱ�
                yield return new WaitForSeconds(SusTime);   //���ӽð� �Ŀ� ����� ��������
                curAddStat.MulAtkDmg -= Figure;             //���ߴ� ���� ����
                break;

            case TakeAct.ASpd:
                CMCcurStat.attackSpd -= Figure;             //���ݼӵ� ��Ÿ�� ����
                yield return new WaitForSeconds(SusTime);   //���ӽð� �Ŀ� ����� ��������
                CMCcurStat.attackSpd += Figure;             //���ݼӵ� ��Ÿ�� �������
                break;

            case TakeAct.Defence:
                CMCcurStat.defPower += (int)Figure;         //���� ����
                yield return new WaitForSeconds(SusTime);   //���ӽð� �Ŀ� ����� ��������
                CMCcurStat.defPower -= (int)Figure;         //���� �������
                break;

            case TakeAct.MDefence:
                CMCcurStat.mdefPower += (int)Figure;        //�������׷� ����
                yield return new WaitForSeconds(SusTime);   //���ӽð� �Ŀ� ����� ��������
                CMCcurStat.mdefPower -= (int)Figure;        //�������׷� �������
                break;

            default:
                yield break;
        }
    }
    #endregion --------------------------- ���� ��Ƽ�� ��ų ---------------------------

    private void OnDisable()
    {
        GameManager.StartEvent -= StartFight;     //����� �̺�Ʈ ����
        StopAllCoroutines();            //�� ������ Ȱ���� ���� �� (�װų� ��ġ ���) ��� �ڷ�ƾ ����
    }    
}
