// Assets/Scripts/UI/GameUIManager.cs
// 게임 UI 전체를 관리하는 클래스 (적, 아군, HP바, 콤보 표시)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    private List<EnemySlot> _enemySlots = new List<EnemySlot>();
    private List<AllyCard> _allyCards = new List<AllyCard>();

    private void Start()
    {
        UpdateHPBar();
        HideCombo();
    }



    // ============================================================
    // 적 슬롯 관리
    // ============================================================

    /// <summary>적 슬롯 생성 (스테이지 시작 시 호출)</summary>
    public void SpawnEnemies(CharacterData[] enemies)
    {
        // 기존 슬롯 제거
        ClearEnemySlots();

        // 적 슬롯 생성
        foreach (var enemyData in enemies)
        {
            if (enemyData == null) continue;

            GameObject slotObj = Instantiate(_enemySlotPrefab, _enemyContainer);
            EnemySlot slot = slotObj.GetComponent<EnemySlot>();
            slot.Initialize(enemyData);
            _enemySlots.Add(slot);
        }
    }

    /// <summary>모든 적 슬롯 제거</summary>
    private void ClearEnemySlots()
    {
        foreach (var slot in _enemySlots)
        {
            Destroy(slot.gameObject);
        }
        _enemySlots.Clear();
    }

    /// <summary>살아있는 적 슬롯 리스트 반환</summary>
    public List<EnemySlot> GetAliveEnemies()
    {
        List<EnemySlot> aliveEnemies = new List<EnemySlot>();
        foreach (var slot in _enemySlots)
        {
            if (slot.IsAlive())
            {
                aliveEnemies.Add(slot);
            }
        }
        return aliveEnemies;
    }

    // ============================================================
    // 아군 카드 관리
    // ============================================================

    /// <summary>아군 카드 생성</summary>
    public void SpawnAllies(CharacterData[] allies)
    {
        // 기존 카드 제거
        ClearAllyCards();

        // 아군 카드 생성
        foreach (var allyData in allies)
        {
            if (allyData == null) continue;

            GameObject cardObj = Instantiate(_allyCardPrefab, _allyContainer);
            AllyCard card = cardObj.GetComponent<AllyCard>();
            card.Initialize(allyData);
            _allyCards.Add(card);
        }
    }

    /// <summary>모든 아군 카드 제거</summary>
    private void ClearAllyCards()
    {
        foreach (var card in _allyCards)
        {
            Destroy(card.gameObject);
        }
        _allyCards.Clear();
    }

    /// <summary>아군 카드 리스트 반환</summary>
    public List<AllyCard> GetAllyCards()
    {
        return _allyCards;
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

    public int GetCurrentHP()
    {
        return _currentHP;
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