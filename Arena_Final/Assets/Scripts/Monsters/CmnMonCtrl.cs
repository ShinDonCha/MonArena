using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

//몬스터들의 움직임이나 스킬사용 여부 등 게임 내에서 계속 변화하는 값들을 컨트롤하는 스크립트
//초기 값 설정 외에 외부에서 변수에 접근할 때 모두 여기를 통함
//각 몬스터들의 전용 스크립트(Zombie,Soldier 등)에 상속된다.
public abstract class CmnMonCtrl : CmnMonSet, IPunObservable
{    
    protected abstract void SkillSetting();       //몬스터의 스킬 확률&쿨타임 설정 함수(몬스터 별로 상이하므로 전용 스크립트에서 설정)    
    private bool attackEnable = true;             //공격 가능한지 판단하는 변수 (공격 딜레이 동안 false상태)
    private readonly Queue<int> actQueue = new(); //발동에 성공한 액티브 스킬 목록
    public GameObject Enemy { get; set; } = null; //공격 타겟

    #region ------------------ PhotonSerializeView ------------------
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)      //로컬과 원격의 정보제공
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
        GameManager.StartEvent += StartFight;       //전투 시작 시 실행할 함수 목록에 추가
        base.Start();
    }

    //Update is called once per frame
    void Update()
    {
        if (photonV != null && !photonV.IsMine)     //멀티 게임일때 IsMine이 아닌 객체면 동작하지 않음
            return;

        if (attackRangeCol.enabled && attackRangeCol.radius < 15.0f)         //공격 대상 추적 콜라이더가 Enable상태이고 정해진 크기보다 작으면
            attackRangeCol.radius += 0.1f;          //SphereCollider 크기 점점 증가시켜서 적 추적 범위 확대

        #region -------------------------- 몬스터 상태 체크, 변경 --------------------------
        switch (monState)
        {
            case MonsterState.Search:   //공격 대상을 찾는 상태
                attackRangeCol.enabled = true;   //공격 목표 탐색을 위한 SphereCollider 활성화
                break;

            case MonsterState.Trace:    //정해진 공격대상을 쫓아가는 상태
                if (Enemy.activeSelf)       //공격대상이 살아있다면
                {
                    if ((Enemy.transform.position - transform.position).magnitude <= CMCcurStat.attackRange)     //공격 사거리 안에 적이 있고
                        if (attackEnable)    //공격 가능한 상태면
                        {
                            AnimReq(MonsterState.Attack.ToString());   //공격상태로 변경
                            navAgent.avoidancePriority = atkPriority;  //네비의 회피우선순위 변경(다른 몬스터가 이 몬스터를 피해가도록 하기위해)
                        }
                        else//(attackEnable) //공격 불가능한 상태면
                        {
                            AnimReq(MonsterState.Rest.ToString());     //휴식 상태로 변경
                        }
                }
                else //(!Enemy.activeSelf) //공격대상이 죽은상태면
                    AnimReq(MonsterState.Search.ToString());    //탐색상태로 변경
                break;

            case MonsterState.Rest:     //일반공격을 실행한 후 잠깐 쉬는 상태
                if (Enemy.activeSelf)        //공격대상이 살아있다면
                {
                    if ((Enemy.transform.position - transform.position).magnitude <= CMCcurStat.attackRange)     //공격 사거리 안에 적이 있고
                    {
                        if (attackEnable)   //공격 가능한 상태면
                            AnimReq(MonsterState.Attack.ToString());    //공격상태로 변경
                    }
                    else//((Enemy.transform.position - transform.position).magnitude > CMCcurStat.attackRange)   //적이 공격 사거리 밖에 있다면
                    {
                        AnimReq(MonsterState.Trace.ToString());         //추적상태로 변경
                        navAgent.avoidancePriority = movePriority;      //네비의 회피우선순위 변경
                    }
                }
                else //(!Enemy.activeSelf)  //공격 대상이 죽은 상태면
                {
                    navAgent.avoidancePriority = movePriority;  //네비의 회피우선순위 변경
                    AnimReq(MonsterState.Search.ToString());    //탐색상태로 변경
                }
                break;

            case MonsterState.Die:     //죽은상태
                if (photonV == null)   //싱글게임일 때
                {
                    ActiveSet(false);   //이 몬스터를 전장에서 사라지게 하기 
                    DeathEffInst();     //사망 이펙트 생성
                    GameMgr.Instance.CalcResults(tag);   //결과 계산을 위해 이 몬스터의 태그 전달
                }
                else    //멀티게임일 때
                {
                    photonV.RPC("ActiveSet", RpcTarget.All, false); //이 몬스터를 전장에서 사라지게 하기
                    photonV.RPC("DeathEffInst", RpcTarget.All);     //사망 이펙트 생성
                    MtGameMgr.Instance.CalcResults(tag);            //결과 계산을 위해 이 몬스터의 태그 전달
                }
                break;
        }
        #endregion -------------------------- 몬스터 상태 체크, 변경 --------------------------
    }

    [PunRPC]
    protected void DeathEffInst()     //사망 이펙트 생성을 위한 RPC
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
    }

    [PunRPC]
    protected void ActiveSet(bool OnOff)    //게임오브젝트의 SetActive 변경을 위한 RPC
    {
        gameObject.SetActive(OnOff);
    }

    private void FixedUpdate()
    {
        if (photonV != null && !photonV.IsMine)     //멀티 게임일때 IsMine이 아닌 객체면 동작하지 않음
            return;

        #region -------------------------- 몬스터 물리연산 --------------------------
        if (Enemy == null || !Enemy.activeSelf)      //공격 대상이 없을경우 취소
            return;

        switch (monState)
        {
            case MonsterState.Trace:            
                transform.rotation = Quaternion.LookRotation(Enemy.transform.position - transform.position);    //공격 대상을 향해 회전
                break;

            case MonsterState.Attack: //공격사거리 안에 공격 대상이 있을때 실행됨 (없을 경우 Update()에서 Trace로 상태가 변경된다.)
                if (attackEnable)     //공격 가능 상태일 때만
                {    
                    Quaternion a_EnemyQ = Quaternion.LookRotation(Enemy.transform.position - transform.position);   //이 몬스터가 대상을 바라볼때의 회전각(쿼터니언)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, a_EnemyQ, rotAngle);          //이 몬스터를 회전 (fixedTime당 최대 rotAngle만큼 회전)
                }                
                break;
        }
        #endregion -------------------------- 몬스터 물리연산 --------------------------
    }

    private void OnTriggerEnter(Collider other)   //콜라이더 충돌 체크
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)) //몬스터의 하위에 있는 MonsterSkin의 Layer가 MonBody
        {
            if (CompareTag(other.transform.parent.tag))  //아군이라면 취소
                return;

            EnemySet(other.transform.parent.gameObject);//충돌한 대상을 공격 목표로 지정
            AnimReq(MonsterState.Trace.ToString());     //추적 애니메이션 재생
            attackRangeCol.enabled = false;             //공격 목표 설정에 사용되는 SphereCollider 비활성화
        }
    }

    protected virtual void StartFight()      //전투 시작 시 실행되는 함수(Mutant만 개인의 StartFight를 사용 중)
    {
        GetComponentInChildren<StCanvasCtrl>(true).gameObject.SetActive(true);     //HP바 표시를 위한 캔버스 On상태로 변경

        if (photonV != null && !photonV.IsMine)     //멀티 원격은 동작하지 않음
            return;

        AnimReq(MonsterState.Search.ToString());    //탐색 애니메이션 실행
    }

    private void AnimReq(string ParamName)    //애니메이션 재생요청 함수 (몬스터의 현재 상태체크를 위한 monState 변수는 각 애니메이션의 시작부분에서 설정된다.)
    {
        if (photonV == null)  //싱글게임일 때
        {
            AnimSet(ParamName);
        }
        else//(photonV != null)     //멀티게임일 때 (AnimReq함수는 IsMine에서만 실행된다.)
        {
            photonV.RPC("AnimSet", RpcTarget.All, ParamName);
        }
    }

    [PunRPC]
    protected void AnimSet(string ParamName) //애니메이션 재생 함수(싱글, 멀티 공용)
    {
        animator.SetTrigger(ParamName);     //해당 애니메이션 재생
        monState = MonsterState.Wait;       //애니메이션 재생 전까지 대기상태로 변경
    }

    public void StateSet(string StateName)   //애니메이션 재생 시 재생중인 애니메이션에 따라 몬스터의 상태를 바꿔주기 위한 함수 (애니메이션의 시작부분에서 이벤트로 실행)
    {
        if (!Enum.TryParse(StateName, out MonsterState MS))
            return;

        monState = MS;      //몬스터 상태 변경

        if (photonV != null && !photonV.IsMine)     //멀티 원격은 동작하지 않음
            return;

        if (MS.Equals(MonsterState.Trace))      //추적 상태로 변경될 예정이라면
        {
            navAgent.isStopped = false;         //네비 정지상태로 변경
            navAgent.destination = Enemy.transform.position;    //이 몬스터의 목적지 변경
        }
        else//(!MS.Equals(MonsterState.Trace))  //그 외 상태로 변경될 예정이라면
            navAgent.isStopped = true;          //네비 활동상태로 변경
    }

    public void EnemySet(GameObject GO)     //공격 목표 설정 함수
    {
        if (photonV == null)        //싱글 플레이
        {
            Enemy = GO;             //매개변수로 들어온 게임 오브젝트를 공격목표로 설정
        }
        else if (photonV != null && photonV.IsMine)     //멀티 플레이(IsMine만 동작)
        {
            if (Enum.TryParse(GO.name[..GO.name.IndexOf("(")], out MonsterName EnemyMN))      //공격대상으로 잡으려는 목표의 이름을 MonsterName으로 변환
                photonV.RPC("MultiEnemySet", RpcTarget.All, EnemyMN);      //로컬과 원격에서 같은 대상을 적으로 하기위한 RPC 실행
        }
    }

    [PunRPC]
    protected void MultiEnemySet(MonsterName EnemyName)  //멀티용 공격 목표 설정 함수
    {
        if (!Enum.TryParse(tag, out Team MyTeamTag))     //이 몬스터의 태그를 Enum형식으로 변경
            return;

        switch (MyTeamTag)
        {
            case Team.Team1:
                Enemy = FindClass.GetMonCMC(Team.Team2.ToString(), EnemyName).gameObject;      //공격대상 지정
                break;

            case Team.Team2:
                Enemy = FindClass.GetMonCMC(Team.Team1.ToString(), EnemyName).gameObject;      //공격대상 지정
                break;
        }
    }    

    #region ----------------------- 공격, 액티브 스킬 -----------------------
    protected virtual void Attack()         //일반 공격 시 동작 (일반 공격 애니메이션 특정부분에서 이벤트로 실행)
    {
        if (photonV != null && !photonV.IsMine)   //멀티 게임일때 IsMine이 아닌 객체면 동작하지 않음
            return;

        StartCoroutine(AttackDelayCalc());  //공격 딜레이 계산
        ActSkillCheck();                    //액티브 스킬발동 체크
        AddUltiGage(15);                    //공격할때 마다 궁극기 게이지 15 획득

        if (UltiGage == 100)                //게이지 다 채웠을 때 궁극 스킬 발동
            AnimReq(Skill.Ultimate.ToString());   //궁극기 애니메이션 재생 요청

        int a_ActSkCount = actQueue.Count;  //발동이 예약된 액티브 스킬의 수 가져오기

        for (int i = 0; i < a_ActSkCount; i++)    //발동이 예약된 액티브 스킬(Skill1, Skill2)이 있을 경우
            AnimReq(((Skill)actQueue.Dequeue()).ToString());     //스킬번호에 맞게 애니메이션 재생
    }

    private IEnumerator AttackDelayCalc()      //일반 공격에 딜레이를 적용시키기 위한 코루틴
    {
        attackEnable = false;       //일반공격 불가능 상태로 변경
        yield return new WaitForSeconds(CMCcurStat.attackSpd);   //공격속도만큼 지난 뒤에 제어권 가져옴
        attackEnable = true;        //일반공격 가능 상태로 변경
    }

    private void ActSkillCheck()            //액티브 스킬 발동 체크 함수(각 스킬마다 발동 확률을 가지고있음)
    {
        for (int i = 0; i < (int)Skill.Count; i++)   //궁극기를 제외한 스킬 개수만큼 실행 (스킬마다 각각 한번씩 발동 확률이 계산되기 때문에 여러개를 같이 발동시킬수도 있음)
        {
            int a_Rnd = UnityEngine.Random.Range(0, 100);

            if (skillOnOff[i])       //해당 스킬을 사용 가능상태면 (스킬쿨타임이 아닐 때 가능상태)
            {
                if (actQueue.Contains(i))      //이미 같은 종류의 액티브 스킬이 등록되어 있을 경우 넘어가기
                    continue;

                if (a_Rnd < SkSet.Prob[i])     //설정한 스킬 발동확률을 만족하면 큐에 추가
                    actQueue.Enqueue(i);
            }
            else//(!skillOnOff[i])  //해당 스킬이 사용 불가능상태면 넘어가기(스킬쿨타임이 아닐 때 가능상태)
                continue;
        }
    }    

    private IEnumerator ActSkillOn(int SkillNum)        //스킬 발동 (Skill1과 Skill2 애니메이션 특정부분에서 이벤트로 실행)
    {
        switch ((Skill)SkillNum)
        {
            case Skill.Sk1:
                Skill1();   //1번 스킬 발동
                break;

            case Skill.Sk2:
                Skill2();   //2번 스킬 발동
                break;
        }

        skillOnOff[SkillNum] = false;  //해당 스킬 사용 불가능 상태로 변경
        yield return new WaitForSeconds(SkSet.Cool[SkillNum]);     //제어권을 돌려받았을 때 SkillNum이 코루틴 시작시 넘겨받은 매개변수에 맞게 값을 가짐, 즉 코루틴마다 다른 넘버 적용
        skillOnOff[SkillNum] = true;   //해당 스킬 사용 가능상태로 변경
    }

    protected virtual void Skill1()     //스킬 1 발동 시 사용되는 스크립트(몬스터 별 전용 스크립트에서 설정)
    {
    }

    protected virtual void Skill2()     //스킬 2 발동 시 사용되는 스크립트(몬스터 별 전용 스크립트에서 설정)
    {
    }
    #endregion ----------------------- 공격, 액티브 스킬 -----------------------

    #region ------------------- 궁극기 관련 -------------------
    private void TimePause()        //궁극기 사용 시 효과를 주기 위한 변수 (궁극기 애니메이션 시작지점에서 이벤트로 실행)
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.UltiReady);     //궁극기 실행 효과음 재생
        Instantiate(ultiEffect, transform);       //궁극기 이펙트 생성
        Time.timeScale = 0.0f;      //일시정지 상태로 변경
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;   //이 유닛의 애니메이션만 재생되도록 모드 변경
        animator.speed = 0.6f;      //애니메이션 속도 조정
    }

    protected virtual void UltiSkill()    //궁극기 발동 함수(궁극기 애니메이션에서 이벤트로 실행)
    {        
        UltiGage = 0;                 //궁극기 게이지 초기화
        Time.timeScale = 1.0f;        //일시정지 해제
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;   //원래 애니메이션 모드로 변경
        animator.speed = 1.0f;        //애니메이션 속도 초기화
    }

    protected void AddUltiGage(int UG)     //궁극기 게이지 충전 함수
    {
        UltiGage += UG;        //게이지 추가

        if (UltiGage > 100)     //최대수치인 100을 넘기지 못하도록
            UltiGage = 100;
    }
    #endregion ------------------- 궁극기 관련 -------------------

    #region -------------------- 대미지 입히기 --------------------
    public int GiveDamage(GameObject Target, int Damage)     //공격 시 상대 스크립트에 접근해서 대미지 적용시키는 함수 (일반공격, 스킬공격 전부 이 스크립트로 대미지 적용)
    {
        if (!Target.activeSelf)     //적이 죽었다면 취소
            return 0;

        Damage = (int)((Damage + curAddStat.AddAtkDmg) * (1.0f + curAddStat.MulAtkDmg));         //현재 공격자가 얻고 있는 대미지 증가량 만큼 총 대미지 증가
        curAddStat.AddAtkDmg = 0;     //추가대미지 초기화

        int a_CalcDmg = Target.GetComponent<CmnMonCtrl>().TakeDamage(CMCcurStat.attackType, Damage);       //적에게 입힐 대미지 계산, 저장
        TotalDmg += a_CalcDmg;        //적에게 입힌 대미지 저장

        return a_CalcDmg;
    }

    private int TakeDamage(AttackType AType, int Damage)       //공격 받았을 때 실행되는 함수(공격자의 공격타입, 대미지)
    {
        if (photonV != null && !photonV.IsMine)
            return 0;

        if (CMCcurStat.hp > 0)          //이 몬스터가 살아있을때만..
        {
            Damage = (int)(Damage * (1.0f + curAddStat.MulTakeDmg));        //받는 피해량 증가분 만큼 피해량 증가

            switch (AType)              //대미지 계산
            {
                case AttackType.Physical:               //적의 공격이 물리 공격일 경우
                    Damage -= CMCcurStat.defPower;      //나의 방어력만큼 감소
                    break;

                case AttackType.Magical:                //적의 공격이 마법 공격일 경우
                    Damage -= CMCcurStat.mdefPower;     //나의 마법저항력 만큼 감소
                    break;
            }

            Damage = Mathf.Max(0, Damage);      //대미지가 음수 나오지 않게 하기 위해

            switch (CMCcurStat.hp > Damage)       //남은 체력과 받을 대미지 비교
            {
                case true: //(CMCcurStat.hp > Damage)
                    if (photonV != null && !photonV.IsMine)     //공격받는 애가 멀티 원격이면 실제 대미지가 들어가기전에 계산한 대미지만 리턴(실제로 피해입는건 로컬에서 적용되도록 하기 위함)
                        return Damage;
                    break;

                case false: //(CMCcurStat.hp <= Damage)
                    Damage = CMCcurStat.hp;

                    if (photonV != null && !photonV.IsMine)     //공격받는 애가 멀티 원격이면 실제 대미지가 들어가기전에 계산한 대미지만 리턴(실제로 피해입는건 로컬에서 적용되도록 하기 위함)
                        return Damage;

                    monState = MonsterState.Die;    //몬스터 사망처리
                    break;
            }

            //-------------- 실제 피해 계산 (공격받는 애가 싱글과 멀티IsMine일 때만 대미지를 입힘)
            CMCcurStat.hp -= Damage; //대미지 입히기
            AddUltiGage(10);         //맞을 때 마다 궁극기 게이지 10씩 얻음
            hpImg.fillAmount = (float)CMCcurStat.hp / CMCmonStat.hp;    //몬스터 머리위의 HP바 표시 변경
            TextRequest.InstantTxtReqAct(transform.position, TxtAnimList.DmgTxtAnim, Damage);       //대미지 텍스트 출력 요청
            TotalHP += Damage;  //받은 총 피해에 추가
            return Damage;      //받은 피해를 공격자에게 전달 (공격자의 대미지 총합에 더해주기 위함)
            //-------------- 실제 피해 계산 (공격받는 애가 싱글과 멀티IsMine일 때만 대미지를 입힘)
        }
        else
        {
            if (!monState.Equals(MonsterState.Die))     //혹시 오류로 죽은상태로 체크되어있지 않다면 변경
                monState = MonsterState.Die;

            return 0;     //이 몬스터가 죽은상태면 대미지 없음
        }
    }
    #endregion -------------------- 대미지 입히기 --------------------    

    #region --------------------------- 받은 액티브 스킬 ---------------------------
    public void TakeAny(TakeAct TA, float Figure, float SusTime = 0.0f)     //받은 액티브 스킬 (종류, 수치, 지속시간) 외부에서 여기로 접근
    {
        if (photonV != null && !photonV.IsMine)     //멀티 게임일때 IsMine이 아닌 객체면 동작하지 않음
            return;

        switch (TA)
        {
            case TakeAct.Heal:      //받은게 힐일 경우
                TakeHeal((int)Figure);
                break;

            default:                //그 외의 경우
                StartCoroutine(BuffTextCo(Figure, SusTime, TA));
                break;            
        }
    }

    private void TakeHeal(int Amount)     //받은 회복 계산 함수
    {
        if ((CMCcurStat.hp + Amount) > CMCmonStat.hp)  //회복으로 최대체력을 넘게되면 최대체력까지만 반영
            Amount = CMCmonStat.hp - CMCcurStat.hp;    //실제 회복량 계산

        CMCcurStat.hp += Amount;         //몬스터 체력 회복
        TextRequest.InstantTxtReqAct(transform.position, TxtAnimList.HealTxtAnim, Amount);       //힐 텍스트 출력 요청
        hpImg.fillAmount = (float)CMCcurStat.hp / CMCmonStat.hp;    //몬스터 머리위의 HP바 표시 변경
    }

    private IEnumerator BuffTextCo(float Figure, float SusTime, TakeAct Act) //증가or감소 수치, 지속시간, 상태변화 종류
    {
        //--------- 들어온 수치에 따라 버프인지 디버프인지 결정
        TxtAnimList a_TxtAList;

        if (Figure > 0)
            a_TxtAList = TxtAnimList.BuffTxtAnim;
        else if (Figure < 0)
            a_TxtAList = TxtAnimList.DeBuffTxtAnim;
        else//(Figure = 0)
            yield break;
        //--------- 들어온 수치에 따라 버프인지 디버프인지 결정

        if(photonV == null)         //싱글
            TextRequest.BuffTxtReqAct(tag, transform.parent.GetSiblingIndex(), transform.position, a_TxtAList, Act);     //상태변화 텍스트 출력
        else//(photonV != null)     //멀티(멀티는 TakeAny가 IsMine만 실행됨)
            TextRequest.BuffTxtReqAct(tag, (int)CMCmonStat.monName, transform.position, a_TxtAList, Act);   //상태변화 텍스트 출력(지금은 5몬스터이라서 가능한데 몬스터 늘어나면 불가능함)

        switch (Act)
        {
            case TakeAct.Dmg:
                curAddStat.MulAtkDmg += Figure;             //대미지 비율에 더하기
                yield return new WaitForSeconds(SusTime);   //지속시간 후에 제어권 돌려받음
                curAddStat.MulAtkDmg -= Figure;             //더했던 비율 빼기
                break;

            case TakeAct.ASpd:
                CMCcurStat.attackSpd -= Figure;             //공격속도 쿨타임 감소
                yield return new WaitForSeconds(SusTime);   //지속시간 후에 제어권 돌려받음
                CMCcurStat.attackSpd += Figure;             //공격속도 쿨타임 원래대로
                break;

            case TakeAct.Defence:
                CMCcurStat.defPower += (int)Figure;         //방어력 증가
                yield return new WaitForSeconds(SusTime);   //지속시간 후에 제어권 돌려받음
                CMCcurStat.defPower -= (int)Figure;         //방어력 원래대로
                break;

            case TakeAct.MDefence:
                CMCcurStat.mdefPower += (int)Figure;        //마법저항력 증가
                yield return new WaitForSeconds(SusTime);   //지속시간 후에 제어권 돌려받음
                CMCcurStat.mdefPower -= (int)Figure;        //마법저항력 원래대로
                break;

            default:
                yield break;
        }
    }
    #endregion --------------------------- 받은 액티브 스킬 ---------------------------

    private void OnDisable()
    {
        GameManager.StartEvent -= StartFight;     //등록한 이벤트 삭제
        StopAllCoroutines();            //이 몬스터의 활동이 끝날 때 (죽거나 배치 취소) 모든 코루틴 종료
    }    
}
