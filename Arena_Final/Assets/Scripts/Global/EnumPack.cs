
//public enum �� ��Ƴ��� ��
public enum Team        //������ ���� �����ֱ� ���� enum
{
    Ally,
    Enemy,
    Team1,      //��Ƽ
    Team2,      //��Ƽ
}

public enum PacketType   //��Ʈ��ũ�� ���� ��û �� ����� ��ŶŸ�� enum
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

public enum MonsterName    //���� �̸��� ǥ���ϱ� ���� enum
{
    Soldier,
    Zombie,
    GamblerCat,
    Mutant,
    Jammo,
    MonsterCount,
    None
}

public enum AttackType     //������ ����Ÿ���� �����ϱ� ���� enum
{
    Physical,
    Magical
}

public enum Skill          //������ ��ų ������ ����ϱ� ���� enum (�̸��� �ִϸ��̼� Ʈ���Ű� ����, �� �� ���� �����ؾ���)
{
    Ultimate = -1,
    Sk1,
    Sk2,
    Count
}

public enum ExplainList     //� ��ų�� ���� ���������� �Ǵ��ϱ� ���� enum, SkillGroup�� ���� ������Ʈ��� �̸��� �����ϱ� ������ ���� ���� �����ؾ���
{
    Ultimate,
    Skill1,
    Skill2
}

public enum Result          //���� ��� ǥ�ÿ� enum
{
    Victory,
    Defeat
}

public enum ButtonList     //��ư�� �̸�
{
    //(�� Scene�� ��ư ������Ʈ �̸��� �����Ƿ� ���� �� ���� ��������� ��, �������̽� IButtonClick�� ButtonOnClick�Լ��� ���)
    //--------- Ÿ��Ʋ
    LoginButton,
    LogoutButton,
    SignupButton,
    CreateButton,
    //--------- Ÿ��Ʋ

    //--------- �κ�
    UserInfoButton,
    CombatButton,
    ShopButton,
    MyRoomButton,
    DefDeckButton,
    ExitButton,
    AutoExpButton,
    //--------- �κ�

    BackButton,    
    StartButton,

    OKButton,       //Ȯ��
    CancelButton,   //���

    NickChangeButton,   //�г��� ����

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

public enum SceneList       //�� ������ ��ü �� ����� ����ִ� enum
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

public enum MonSlotTag      //MonSlot�� Tag(���� ���� Tag�� ��ġ���Ѿ���)
{
    Untagged,
    Content,
    Drag,
    NameParse,
}

public enum TakeAct       //���Ͱ� ���� ��Ƽ�� ��ų (��, ����, ����� ��)
{
    Heal,       //��
    Dmg,        //����� ����&�����
    ASpd,       //���ݼӵ� ����&�����
    Defence,    //���� ����&�����
    MDefence,   //�������׷� ����&�����
}

public enum TxtAnimList     //����&����� �ؽ�Ʈ ��� �ִϸ��̼� �̸�
{
    BuffTxtAnim,
    DeBuffTxtAnim,
    DmgTxtAnim,
    HealTxtAnim,
}

public enum LayerName       //������� Layer�� �̸�(Layer��ȣ��� �����������)
{    
    SetPoint = 6,
    MonSlot,
    MonBody,
    Terrain,
    Wall,
}

public enum BGMList        //BGM ���
{
    Title,
    Lobby,
    Battle,    
    RankLobby,
    Stop,
}

public enum EffSoundList    //ȿ���� ���
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


