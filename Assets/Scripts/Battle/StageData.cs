// Assets/Scripts/Battle/StageData.cs
// 스테이지 정보를 정의하는 ScriptableObject (적 배치)

using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "PuzzleDragon/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("스테이지 정보")]
    public int stageNumber = 1;
    public string stageName = "스테이지 1";

    [Header("적 배치 (최대 3마리)")]
    public CharacterData[] enemies = new CharacterData[3];
}