// Assets/Scripts/Managers/GameManager.cs
// 게임 전체 흐름을 관리하는 메인 매니저

using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private GameUIManager _gameUIManager;

    [Header("게임 데이터")]
    [SerializeField] private StageData _currentStage;
    [SerializeField] private CharacterData[] _playerTeam = new CharacterData[5];

    private void Start()
    {
        InitializeGame();
    }

    /// <summary>게임 초기화 - UI 세팅</summary>
    private void InitializeGame()
    {
        // 적 스폰
        if (_currentStage != null)
        {
            _gameUIManager.SpawnEnemies(_currentStage.enemies);
        }

        // 아군 스폰
        _gameUIManager.SpawnAllies(_playerTeam);

        // 플레이어 HP 설정
        _gameUIManager.SetMaxHP(1000);
    }
}