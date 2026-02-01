// Assets/Scripts/Input/InputManager.cs
// 드래그 앤 드롭 입력을 처리하고 구슬 교환을 담당하는 클래스
// PC(마우스) / 모바일(터치)을 플랫폼 디파인으로 분리
// 드래그 구슬은 인접한 셀(상/하/좌/우)과만 교환 가능

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private MatchDetector _matchDetector;
    [SerializeField] private BoardCleaner _boardCleaner;

    /// <summary>현재 드래그 중인 구슬</summary>
    private Orb _selectedOrb;

    /// <summary>드래그 구슬이 현재 차지하고 있는 보드 좌표</summary>
    private int _currentRow;
    private int _currentCol;

    /// <summary>드래그 중 카메라와의 Z 거리</summary>
    private float _zDistance;

    /// <summary>현재 제거/리필 처리 중인지 여부 (중복 입력 방지)</summary>
    private bool _isProcessing;

    private void Start()
    {
        if (_boardManager == null)
            Debug.LogError("InputManager: BoardManager 참조가 비어있습니다.");
        if (_matchDetector == null)
            Debug.LogError("InputManager: MatchDetector 참조가 비어있습니다.");
        if (_boardCleaner == null)
            Debug.LogError("InputManager: BoardCleaner 참조가 비어있습니다.");
    }

    private void Update()
    {
        // 처리 중이면 입력 무시
        if (_isProcessing) return;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
        HandleMouseInput();
#elif UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
#endif
    }

    // ============================================================
    // PC 마우스 입력 처리
    // ============================================================
    private void HandleMouseInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnPointerDown(Mouse.current.position.ReadValue());
        }
        else if (Mouse.current.leftButton.isPressed && _selectedOrb != null)
        {
            OnPointerMove(Mouse.current.position.ReadValue());
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && _selectedOrb != null)
        {
            OnPointerUp();
        }
    }

    // ============================================================
    // 모바일 터치 입력 처리
    // ============================================================
    private void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        for (int i = 0; i < Touchscreen.current.touches.Count; i++)
        {
            var touch = Touchscreen.current.touches[i];
            var phase = touch.phase.ReadValue();

            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                OnPointerDown(touch.position.ReadValue());
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Moved && _selectedOrb != null)
            {
                OnPointerMove(touch.position.ReadValue());
            }
            else if ((phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                      phase == UnityEngine.InputSystem.TouchPhase.Canceled) && _selectedOrb != null)
            {
                OnPointerUp();
            }
        }
    }

    // ============================================================
    // 공통 포인터 이벤트 처리
    // ============================================================

    /// <summary>포인터 누름 - 구슬 선택</summary>
    private void OnPointerDown(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            Orb orb = hit.collider.GetComponent<Orb>();
            if (orb != null)
            {
                _selectedOrb = orb;
                _currentRow = orb.Row;
                _currentCol = orb.Col;
                _zDistance = -Camera.main.transform.position.z;

                // 드래그 중 다른 구슬 위로 올라오도록 Z축 조정
                _selectedOrb.transform.position += new Vector3(0, 0, -0.1f);

                // 드래그 중 충돌 비활성화
                _selectedOrb.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    /// <summary>포인터 이동 - 구슬을 따라 이동 및 인접 교환</summary>
    private void OnPointerMove(Vector2 screenPos)
    {
        // 드래그 구슬을 마우스/터치 위치로 따라 이동
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _zDistance));
        _selectedOrb.transform.position = new Vector3(worldPos.x, worldPos.y, _selectedOrb.transform.position.z);

        // 드래그 구슬의 현재 월드 좌표를 가장 가까운 보드 셀로 변환
        int nearestRow, nearestCol;
        GetNearestCell(_selectedOrb.transform.position, out nearestRow, out nearestCol);

        // 현재 차지하고 있는 셀과 다른 셀로 이동했고, 그 셀이 인접한지 확인
        if (nearestRow != _currentRow || nearestCol != _currentCol)
        {
            if (IsAdjacent(_currentRow, _currentCol, nearestRow, nearestCol))
            {
                // 인접한 셀의 구슬과 교환
                SwapOrbs(_currentRow, _currentCol, nearestRow, nearestCol);

                // 드래그 구슬의 현재 보드 좌표 갱신
                _currentRow = nearestRow;
                _currentCol = nearestCol;
            }
        }
    }

    /// <summary>포인터 해제 - 드래그 종료 후 매칭 감지 및 제거/리필 처리</summary>
    private void OnPointerUp()
    {
        if (_selectedOrb == null) return;

        // 충돌 다시 활성화
        _selectedOrb.GetComponent<Collider2D>().enabled = true;

        // 현재 차지 중인 셀 위치로 스냅
        Vector3 snapPos = _boardManager.GetCellPosition(_currentRow, _currentCol);
        _selectedOrb.transform.position = new Vector3(snapPos.x, snapPos.y, 0f);

        _selectedOrb = null;

        // 드래그 종료 후 매칭 감지 및 제거/리필 처리
        if (_matchDetector == null || _boardCleaner == null) return;

        _isProcessing = true;
        StartCoroutine(_boardCleaner.ProcessMatches());
        StartCoroutine(WaitForProcessing());
    }

    /// <summary>BoardCleaner 처리 완료 대기</summary>
    private System.Collections.IEnumerator WaitForProcessing()
    {
        yield return new WaitUntil(() => !_boardCleaner.IsProcessing);
        _isProcessing = false;
    }

    // ============================================================
    // 인접 판정 & 교환 로직
    // ============================================================

    /// <summary>두 좌표가 상/하/좌/우 인접한지 판정 (맨해튼 거리 == 1)</summary>
    private bool IsAdjacent(int row1, int col1, int row2, int col2)
    {
        int dist = Mathf.Abs(row1 - row2) + Mathf.Abs(col1 - col2);
        return dist == 1;
    }

    /// <summary>월드 좌표를 가장 가까운 보드 셀 좌표로 변환</summary>
    private void GetNearestCell(Vector3 worldPos, out int row, out int col)
    {
        int rows = _boardManager.Board.GetLength(0);
        int cols = _boardManager.Board.GetLength(1);

        float minDist = float.MaxValue;
        row = 0;
        col = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 cellPos = _boardManager.GetCellPosition(r, c);
                float dist = Vector3.Distance(worldPos, cellPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    row = r;
                    col = c;
                }
            }
        }
    }

    /// <summary>두 셀의 구슬을 교환 (보드 배열 + 위치 + 좌표 모두 갱신)</summary>
    private void SwapOrbs(int row1, int col1, int row2, int col2)
    {
        Orb orb1 = _boardManager.Board[row1, col1];
        Orb orb2 = _boardManager.Board[row2, col2];

        // 보드 배열 교환
        _boardManager.Board[row1, col1] = orb2;
        _boardManager.Board[row2, col2] = orb1;

        // 드래그 중인 구슬이 아닌 쪽만 위치 이동 (드래그 구슬은 마우스로 움직이기 때문)
        if (orb1 == _selectedOrb)
        {
            orb2.transform.position = _boardManager.GetCellPosition(row1, col1);
        }
        else
        {
            orb1.transform.position = _boardManager.GetCellPosition(row2, col2);
        }

        // 보드 좌표 교환
        orb1.Row = row2;
        orb1.Col = col2;
        orb2.Row = row1;
        orb2.Col = col1;
    }
}