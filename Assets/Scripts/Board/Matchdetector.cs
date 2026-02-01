// Assets/Scripts/Board/MatchDetector.cs
// 보드에서 같은 타입의 구슬이 가로/세로 3개 이상 연결되었는지 감지하는 클래스

using UnityEngine;
using System.Collections.Generic;

public class MatchDetector : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private BoardManager _boardManager;

    /// <summary>
    /// 보드 전체를 탐색하여 매칭된 구슬 리스트를 반환
    /// </summary>
    public List<Orb> DetectMatches()
    {

        Debug.Log("[MatchDetector] DetectMatches 호출됨");

        if (_boardManager == null)
        {
            Debug.LogError("[MatchDetector] BoardManager가 null입니다!");
            return new List<Orb>();
        }

        if (_boardManager.Board == null)
        {
            Debug.LogError("[MatchDetector] Board 배열이 null입니다!");
            return new List<Orb>();
        }
        

        // HashSet으로 중복 방지
        HashSet<Orb> matchedSet = new HashSet<Orb>();

        int rows = _boardManager.Board.GetLength(0);
        int cols = _boardManager.Board.GetLength(1);

        // 가로 매칭 탐색
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols - 2; col++)
            {
                Orb current = _boardManager.Board[row, col];
                Orb right1 = _boardManager.Board[row, col + 1];
                Orb right2 = _boardManager.Board[row, col + 2];

                if (current.OrbType == right1.OrbType && current.OrbType == right2.OrbType)
                {
                    matchedSet.Add(current);
                    matchedSet.Add(right1);
                    matchedSet.Add(right2);
                }
            }
        }

        // 세로 매칭 탐색
        for (int row = 0; row < rows - 2; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Orb current = _boardManager.Board[row, col];
                Orb down1 = _boardManager.Board[row + 1, col];
                Orb down2 = _boardManager.Board[row + 2, col];

                if (current.OrbType == down1.OrbType && current.OrbType == down2.OrbType)
                {
                    matchedSet.Add(current);
                    matchedSet.Add(down1);
                    matchedSet.Add(down2);
                }
            }
        }

        return new List<Orb>(matchedSet);
    }
}