// Assets/Scripts/UI/EnemySlot.cs
// 적 1마리를 표시하는 UI 슬롯

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySlot : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private TextMeshProUGUI _turnText;

    private CharacterData _enemyData;
    private int _currentHP;
    private int _currentTurn;

    /// <summary>적 데이터 설정 및 UI 초기화</summary>
    public void Initialize(CharacterData enemyData)
    {
        _enemyData = enemyData;
        _currentHP = enemyData.maxHP;
        _currentTurn = enemyData.turnCount;

        // 아이콘 설정
        if (_iconImage != null && enemyData.icon != null)
        {
            _iconImage.sprite = enemyData.icon;
            _iconImage.color = OrbColorMap.GetColor(enemyData.attribute);
        }

        UpdateUI();
    }

    /// <summary>데미지를 받음</summary>
    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        if (_currentHP < 0) _currentHP = 0;
        UpdateUI();
    }

    /// <summary>턴 감소 (플레이어 공격 후 호출)</summary>
    public void DecreaseTurn()
    {
        _currentTurn--;
        if (_currentTurn < 0) _currentTurn = 0;
        UpdateUI();
    }

    /// <summary>적이 공격 (턴이 0이 되면 호출)</summary>
    public int Attack()
    {
        if (_currentTurn == 0)
        {
            _currentTurn = _enemyData.turnCount; // 턴 리셋
            UpdateUI();
            return _enemyData.damage;
        }
        return 0;
    }

    /// <summary>적이 살아있는지 여부</summary>
    public bool IsAlive()
    {
        return _currentHP > 0;
    }

    /// <summary>UI 갱신</summary>
    private void UpdateUI()
    {
        // HP 바
        if (_hpSlider != null)
        {
            _hpSlider.value = (float)_currentHP / _enemyData.maxHP;
        }

        // 턴 텍스트
        if (_turnText != null)
        {
            _turnText.text = _currentTurn.ToString();
        }
    }
}