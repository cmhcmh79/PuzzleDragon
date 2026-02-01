using UnityEngine;

/// <summary>
/// 개별 구슬을 나타내는 컴포넌트.
/// OrbType을 설정하면 색상이 자동으로 적용됩니다.
/// </summary>
public class Orb : MonoBehaviour
{
    [Header("구슬 설정")]
    [SerializeField] private OrbType _orbType;

    private SpriteRenderer _spriteRenderer;

    /// <summary>구슬 타입 (변경 시 색상 자동 업데이트)</summary>
    public OrbType OrbType
    {
        get => _orbType;
        set => SetOrbType(value);
    }

    /// <summary>보드 내 위치 (행, 열)</summary>
    public int Row { get; set; }
    public int Col { get; set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Start에서 초기 색상 적용
    /// </summary>
    private void Start()
    {
        ApplyColor();
    }

    /// <summary>
    /// OrbType을 세팅하고 색상을 갱신
    /// </summary>
    private void SetOrbType(OrbType type)
    {
        _orbType = type;
        ApplyColor();
    }

    /// <summary>
    /// 현재 OrbType에 맞는 색상을 SpriteRenderer에 적용
    /// </summary>
    private void ApplyColor()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = OrbColorMap.GetColor(_orbType);
        }
    }
}