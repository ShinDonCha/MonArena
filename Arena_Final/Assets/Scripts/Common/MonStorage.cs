using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�̹����� ������Ʈ���� �ҷ����� ���� ScriptableObject
[CreateAssetMenu(fileName = "MonStorage", menuName = "ScriptableObject/MonStorage", order = int.MaxValue)]
public class MonStorage : ScriptableObject
{
    public Sprite[] characterImg = new Sprite[20];      //ĳ���� �̹���

    public Sprite[] monstersImg = new Sprite[(int)MonsterName.MonsterCount];        //���� �̹���

    public GameObject[] monstersObj = new GameObject[(int)MonsterName.MonsterCount];    //���� ������Ʈ

    public GameObject[] monstersSkGroup = new GameObject[(int)MonsterName.MonsterCount];    //���� ��ų��

    public GameObject[] monSlot = new GameObject[6];    //���� ���� ������
}
