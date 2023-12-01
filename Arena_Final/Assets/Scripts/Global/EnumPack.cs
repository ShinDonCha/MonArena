
//public enum 을 모아놓은 곳
public enum Team        //몬스터의 팀을 정해주기 위한 enum
{
    Ally,
    Enemy,
    Team1,      //멀티
    Team2,      //멀티
}

public enum PacketType   //네트워크에 정보 요청 시 사용할 패킷타입 enum
{
    NickName,
    UserCrt,
    UserLv,
    MonList,
    AutoTime,
    UserGold,
    CombatStage,
    CombatDeck,
    DefDeck,
    RankDeck,
    Ranking,
}

public enum MonsterName    //몬스터 이름을 표시하기 위한 enum
{
    Soldier,
    Zombie,
    GamblerCat,
    Mutant,
    Jammo,
    MonsterCount,
    None
}

public enum AttackType     //몬스터의 공격타입을 선택하기 위한 enum
{
    Physical,
    Magical
}

public enum Skill          //몬스터의 스킬 정보에 사용하기 위한 enum (이름과 애니메이션 트리거가 같음, 둘 다 같이 수정해야함)
{
    Ultimate = -1,
    Sk1,
    Sk2,
    Count
}

public enum ExplainList     //어떤 스킬에 대한 설명인지를 판단하기 위한 enum, SkillGroup의 하위 오브젝트들과 이름을 공유하기 때문에 둘이 같이 수정해야함
{
    Ultimate,
    Skill1,
    Skill2
}

public enum Result          //전투 결과 표시용 enum
{
    Victory,
    Defeat
}

public enum ButtonList     //버튼의 이름
{
    //(각 Scene의 버튼 오브젝트 이름과 같으므로 수정 시 같이 수정해줘야 함, 인터페이스 IButtonClick의 ButtonOnClick함수에 사용)
    //--------- 타이틀
    LoginButton,
    LogoutButton,
    SignupButton,
    CreateButton,
    //--------- 타이틀

    //--------- 로비
    UserInfoButton,
    CombatButton,
    ShopButton,
    MyRoomButton,
    DefDeckButton,
    ExitButton,
    AutoExpButton,
    //--------- 로비

    BackButton,    
    StartButton,

    OKButton,       //확인
    CancelButton,   //취소

    NickChangeButton,   //닉네임 변경

    //------- MonInform
    LeftButton,
    RightButton,
    //------- MonInform

    EvolveButton,

    ConfirmButton,
    RetryButton,

    OnceSumButton,
    FiveTSumButton,

    ChallengeButton,

    MultiButton,

    CreatRoomButton,
    KickButton,

    ResultPanel,
    RankLobbyButton,
}

public enum SceneList       //이 게임의 전체 씬 목록을 담고있는 enum
{
    TitleScene,
    LobbyScene,
    StageScene,
    InGameScene,
    ResultScene,
    ShopScene,
    MyRoomScene,
    MonInformScene,
    RankLobbyScene,
    RankGameScene,
    DefDeckScene,
    EvolveScene,
    MultiLobbyScene,
    MultiGameScene,
    MultiResultScene
}

public enum MonSlotTag      //MonSlot의 Tag(게임 내의 Tag랑 일치시켜야함)
{
    Untagged,
    Content,
    Drag,
    NameParse,
}

public enum TakeAct       //몬스터가 받은 액티브 스킬 (힐, 버프, 디버프 등)
{
    Heal,       //힐
    Dmg,        //대미지 버프&디버프
    ASpd,       //공격속도 버프&디버프
    Defence,    //방어력 버프&디버프
    MDefence,   //마법저항력 버프&디버프
}

public enum TxtAnimList     //버프&디버프 텍스트 출력 애니메이션 이름
{
    BuffTxtAnim,
    DeBuffTxtAnim,
    DmgTxtAnim,
    HealTxtAnim,
}

public enum LayerName       //사용중인 Layer의 이름(Layer번호대로 설정해줘야함)
{    
    SetPoint = 6,
    MonSlot,
    MonBody,
    Terrain,
    Wall,
}

public enum BGMList        //BGM 목록
{
    Title,
    Lobby,
    Battle,    
    RankLobby,
    Stop,
}

public enum EffSoundList    //효과음 목록
{
    ButtonClick,
    ResultVictory,
    ResultDefeat,
    MonCreate,
    Fight,
    fail,
    UltiReady,
    Shop,
    Evolve,
}


