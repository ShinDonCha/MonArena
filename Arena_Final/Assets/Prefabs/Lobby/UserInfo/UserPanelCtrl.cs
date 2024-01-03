using UnityEngine;
using UnityEngine.UI;
using System;

//유저의 현재 정보를 표시해 주는 스크립트
//UserInfoPanel 프리팹에 붙여서 사용
public class UserPanelCtrl : NetworkMgr, IButtonClick
{
    [SerializeField]
    private MonStorage monStore = null;     //이미지,오브젝트 저장소

    [Header("-------- LeftBackGround --------")]
    //---------------- 현재 정보
    [SerializeField]
    private Text userUIDText = null;             //유저의 UniqueID 표시용 텍스트
    [SerializeField]
    private Image userCrtImage = null;          //유저의 현재 캐릭터 이미지
    [SerializeField]
    private Text userLevelText = null;          //유저의 레벨 표시용 텍스트
    [SerializeField]
    private Text userNickText = null;            //닉네임 표시용 텍스트
    //---------------- 현재 정보

    //---------------- 프리팹
    [SerializeField]
    private GameObject nickPanelPrefab = null;  //닉네임 변경 판넬 프리팹
    //---------------- 프리팹

    private void Awake()
    {
        userCrtImage.sprite = monStore.characterImg[PlayerInfo.UserCrtNum]; //유저의 캐릭터 이미지 표시  
        userUIDText.text = "UID : " + PlayerInfo.UniqueID.ToString();       //유저의 고유ID표시
        userNickText.text = "닉네임 : " + PlayerInfo.UserNick;               //유저의 닉네임 표시
        userLevelText.text = "레벨 : " + PlayerInfo.UserLevel.ToString();    //유저의 레벨 표시
    }

    protected override void Update()
    {
        base.Update();
    }

    public void ButtonOnClick(Button PushBtn)       //Hierarchy의 버튼 오브젝트들의 OnClick()을 통해 실행
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch(BtnL)
        {
            case ButtonList.NickChangeButton:               //닉네임 변경 클릭 시
                Instantiate(nickPanelPrefab, transform);    //닉네임 변경 판넬 생성
                break;

            case ButtonList.CancelButton:       //취소버튼 클릭 시
                StopAllCoroutines();            //진행중인 코루틴(NetworkMgr에서 진행되는 저장 코루틴) 모두 종료
                Destroy(gameObject);            //이 오브젝트(유저 정보판넬) 삭제
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneList.LobbyScene.ToString());   //로비씬 다시 불러오기 (닉네임 변경하거나 초상화 변경했을때 바로 적용해주기 위함)
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }

    public void CrtChange(int ChrNum)     //캐릭터 이미지 변경 함수(하위의 CharacterImgTab의 CrtImgTabCtrl 스크립트에서 호출)
    {
        PlayerInfo.UserCrtNum = ChrNum;     //바뀐 이미지 번호로 저장
        PushPacket(PacketType.UserCrt);     //변경된 이미지번호 저장 요청
        userCrtImage.sprite = monStore.characterImg[ChrNum];    //캐릭터 이미지 변경
    }

    public void ReqNickCheck()   //닉네임 확인 함수(닉네임 변경 판넬의 NickPanelCtrl에서 실행)
    {
        PushPacket(PacketType.NickName);    //닉네임 확인&저장 요청
    }

    public void NickChange(string Nick)     //닉네임 변경 함수(NetworkMgr에서 중복 닉네임 검사가 끝나면 실행)
    {
        PlayerInfo.UserNick = Nick;                 //닉네임 정적변수 변경
        userNickText.text = "닉네임 : " + Nick;      //현재 닉네임 표시 변경
    }
}
