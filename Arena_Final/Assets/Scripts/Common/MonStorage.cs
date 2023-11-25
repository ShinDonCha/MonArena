using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이미지와 오브젝트들을 불러오기 위한 ScriptableObject
[CreateAssetMenu(fileName = "MonStorage", menuName = "ScriptableObject/MonStorage", order = int.MaxValue)]
public class MonStorage : ScriptableObject
{
    public Sprite[] characterImg = new Sprite[20];      //캐릭터 이미지

    public Sprite[] monstersImg = new Sprite[(int)MonsterName.MonsterCount];        //몬스터 이미지

    public GameObject[] monstersObj = new GameObject[(int)MonsterName.MonsterCount];    //몬스터 오브젝트

    public GameObject[] monstersSkGroup = new GameObject[(int)MonsterName.MonsterCount];    //몬스터 스킬판

    public GameObject[] monSlot = new GameObject[6];    //몬스터 슬롯 프리팹
}
