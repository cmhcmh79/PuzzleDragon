// Assets/Scripts/Board/BoardCleaner.cs
// 매칭된 구슬 제거, 중력(아래로 떨어짐), 리필, 연쇄 매칭(콤보) 처리를 담당하는 클래스

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardCleaner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private MatchDetector _matchDetector;

    [Header("타이밍 설정")]
    [SerializeField] private float _removeDelay = 0.3f;   // 구슬 제거 전 대기 시간
    [SerializeField] private float _gravitySpeed = 8.0f;  // 중력 이동 속도
    [SerializeField] private float _refillDelay = 0.2f;   // 리필 전 대기 시간

    /// <summary>현재 처리 중인지 여부</summary>
    public bool IsProcessing { get; private set; }

    /// <summary>현재 콤보 수</summary>
    private int _comboCount;

    /// <summary>
    /// 매칭 감지 → 제거 → 중력 → 리필 → 연쇄 매칭을 반복하는 메인 처리
    /// </summary>
    public IEnumerator ProcessMatches()
    {
        IsProcessing = true;
        _comboCount = 0;

        while (true)
        {
            // 매칭 감지
            List<Orb> matches = _matchDetector.DetectMatches();

            // 매칭 없으면 종료
            if (matches.Count == 0) break;

            _comboCount++;
            Debug.Log($"콤보 {_comboCount}! 매칭된 구슬 {matches.Count}개");

            // 제거 전 잠깐 대기 (시각적 확인용)
            yield return new WaitForSeconds(_removeDelay);

            // 매칭된 구슬 제거
            RemoveOrbs(matches);

            // 중력 처리 완료 대기
            yield return StartCoroutine(ApplyGravity());

            // 리필 전 잠깐 대기
            yield return new WaitForSeconds(_refillDelay);

            // 빈 공간 리필
            RefillBoard();
        }

        IsProcessing = false;
    }

    // ============================================================
    // 구슬 제거
    // ============================================================

    /// <summary>매칭된 구슬들을 보드에서 제거</summary>
    private void RemoveOrbs(List<Orb> matches)
    {
        foreach (Orb orb in matches)
        {
            // 보드 배열에서 null로 세팅
            _boardManager.Board[orb.Row, orb.Col] = null;

            // GameObject 삭제
            Destroy(orb.gameObject);
        }
    }

    // ============================================================
    // 중력 처리 (위의 구슬이 아래로 떨어짐)
    // ============================================================

    /// <summary>각 열에서 빈 공간이 있으면 위의 구슬들을 아래로 떨어뜨림</summary>
    private IEnumerator ApplyGravity()
    {
        int rows = _boardManager.Board.GetLength(0);
        int cols = _boardManager.Board.GetLength(1);
        bool anyMoved = true;

        while (anyMoved)
        {
            anyMoved = false;

            for (int col = 0; col < cols; col++)
            {
                // 아래부터 탐색하여 빈 공간 찾기
                for (int row = rows - 1; row > 0; row--)
                {
                    if (_boardManager.Board[row, col] == null)
                    {
                        // 위쪽에서 구슬 찾기
                        for (int upperRow = row - 1; upperRow >= 0; upperRow--)
                        {
                            if (_boardManager.Board[upperRow, col] != null)
                            {
                                // 구슬을 아래로 이동
                                Orb fallingOrb = _boardManager.Board[upperRow, col];

                                _boardManager.Board[row, col] = fallingOrb;
                                _boardManager.Board[upperRow, col] = null;

                                fallingOrb.Row = row;
                                fallingOrb.Col = col;

                                // 부드럽게 이동
                                Vector3 targetPos = _boardManager.GetCellPosition(row, col);
                                yield return StartCoroutine(MoveOrb(fallingOrb, targetPos));

                                anyMoved = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>구슬을 타겟 위치로 부드럽게 이동</summary>
    private IEnumerator MoveOrb(Orb orb, Vector3 targetPos)
    {
        while (Vector3.Distance(orb.transform.position, targetPos) > 0.01f)
        {
            orb.transform.position = Vector3.MoveTowards(orb.transform.position, targetPos, _gravitySpeed * Time.deltaTime);
            yield return null;
        }
        orb.transform.position = targetPos;
    }

    // ============================================================
    // 리필
    // ============================================================

    /// <summary>빈 공간(null)에 새로운 랜덤 구슬 생성</summary>
    private void RefillBoard()
    {
        int rows = _boardManager.Board.GetLength(0);
        int cols = _boardManager.Board.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (_boardManager.Board[row, col] == null)
                {
                    // 랜덤 OrbType 선택
                    OrbType randomType = (OrbType)Random.Range(0, System.Enum.GetValues(typeof(OrbType)).Length);

                    // 해당 열의 맨 위에서 생성
                    Vector3 spawnPos = _boardManager.GetCellPosition(0, col);
                    spawnPos.y += _boardManager.Board.GetLength(0) * 0.75f; // 보드 위쪽에서 생성

                    GameObject orbObj = Instantiate(_boardManager.OrbPrefab, spawnPos, Quaternion.identity, _boardManager.transform);
                    orbObj.name = $"Orb_{row}_{col}";

                    Orb orb = orbObj.GetComponent<Orb>();
                    orb.OrbType = randomType;
                    orb.Row = row;
                    orb.Col = col;

                    _boardManager.Board[row, col] = orb;

                    // 생성된 위치에서 실제 위치로 바로 이동 (중력이 자동으로 처리)
                    Vector3 targetPos = _boardManager.GetCellPosition(row, col);
                    orb.transform.position = targetPos;
                }
            }
        }
    }
}