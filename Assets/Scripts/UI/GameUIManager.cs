// Assets/Scripts/UI/GameUIManager.cs
// 게임 UI 전체를 관리하는 클래스 (적, 아군, HP바, 콤보 표시)

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("적 UI (상단)")]
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private GameObject _enemySlotPrefab;

    [Header("아군 UI (중상단)")]
    [SerializeField] private Transform _allyContainer;
    [SerializeField] private GameObject _allyCardPrefab;

    [Header("HP 바 (중하단)")]
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private TextMeshProUGUI _hpText;

    [Header("콤보 표시")]
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private CanvasGroup _comboCanvasGroup;

    private int _maxHP = 1000;
    private int _currentHP = 1000;

    private void Start()
    {
        UpdateHPBar();
        HideCombo();
    }

    // ============================================================
    // HP 바 관리
    // ============================================================

    public void SetMaxHP(int maxHP)
    {
        _maxHP = maxHP;
        _currentHP = maxHP;
        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        if (_currentHP < 0) _currentHP = 0;
        UpdateHPBar();
    }

    public void Heal(int amount)
    {
        _currentHP += amount;
        if (_currentHP > _maxHP) _currentHP = _maxHP;
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (_hpSlider != null)
        {
            _hpSlider.value = (float)_currentHP / _maxHP;
        }

        if (_hpText != null)
        {
            _hpText.text = $"{_currentHP} / {_maxHP}";
        }
    }

    // ============================================================
    // 콤보 표시
    // ============================================================

    public void ShowCombo(int comboCount)
    {
        if (_comboText != null)
        {
            _comboText.text = $"{comboCount} COMBO!";
            _comboCanvasGroup.alpha = 1f;
        }
    }

    public void HideCombo()
    {
        if (_comboCanvasGroup != null)
        {
            _comboCanvasGroup.alpha = 0f;
        }
    }
}