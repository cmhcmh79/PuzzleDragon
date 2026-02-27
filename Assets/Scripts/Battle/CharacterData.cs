// Assets/Scripts/Battle/CharacterData.cs
// 캐릭터(아군/적) 데이터를 정의하는 ScriptableObject

using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "PuzzleDragon/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("기본 정보")]
    public string characterName = "캐릭터";
    public OrbType attribute;           // 속성 (불/물/나무/빛/어둠)
    public Sprite icon;                 // 캐릭터 아이콘

    [Header("스탯")]
    public int maxHP = 100;
    public int attack = 50;
    public int defense = 10;

    [Header("적 전용")]
    public int turnCount = 3;           // 공격까지 남은 턴
    public int damage = 30;             // 적 공격 데미지
}