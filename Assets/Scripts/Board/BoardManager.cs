// Assets/Scripts/Board/BoardManager.cs
// 5×6 퍼즐 보드를 생성하고 관리하는 클래스

using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("보드 설정")]
    [SerializeField] private int _rows = 5;          // 행 (세로)
    [SerializeField] private int _cols = 6;          // 열 (가로)

    [Header("구슬 설정")]
    [SerializeField] private GameObject _orbPrefab;  // 구슬 프리팹
    [SerializeField] private float _cellSize = 0.75f; // 셀 간격

    [Header("보드 위치 설정")]
    [SerializeField] private Vector2 _boardOffset = new Vector2(0f, -1.5f); // 보드 중앙 오프셋

    /// <summary>보드의 구슬 배열 [행, 열]</summary>
    public Orb[,] Board { get; private set; }

    /// <summary>구슬 프리팹 (BoardCleaner 리필에서 사용)</summary>
    public GameObject OrbPrefab => _orbPrefab;

    private void Start()
    {
        GenerateBoard();
    }

    /// <summary>
    /// 보드를 생성하고 랜덤 구슬로 채움
    /// </summary>
    private void GenerateBoard()
    {
        Board = new Orb[_rows, _cols];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                // 랜덤 OrbType 선택
                OrbType randomType = (OrbType)Random.Range(0, System.Enum.GetValues(typeof(OrbType)).Length);

                // 구슬 생성
                Vector3 pos = GetCellPosition(row, col);
                GameObject orbObj = Instantiate(_orbPrefab, pos, Quaternion.identity, transform);
                orbObj.name = $"Orb_{row}_{col}";

                // Orb 컴포넌트 설정
                Orb orb = orbObj.GetComponent<Orb>();
                orb.OrbType = randomType;
                orb.Row = row;
                orb.Col = col;

                Board[row, col] = orb;
            }
        }
    }

    /// <summary>
    /// 행, 열 좌표를 월드 포지션으로 변환
    /// </summary>
    public Vector3 GetCellPosition(int row, int col)
    {
        // 보드를 중앙 기준으로 배치
        float x = (col - (_cols - 1) / 2.0f) * _cellSize + _boardOffset.x;
        float y = ((_rows - 1) / 2.0f - row) * _cellSize + _boardOffset.y; // row 0이 위
        return new Vector3(x, y, 0f);
    }
}