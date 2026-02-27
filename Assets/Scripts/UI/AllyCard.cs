// Assets/Scripts/UI/AllyCard.cs
// 아군 1명을 표시하는 UI 카드

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AllyCard : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;

    private CharacterData _allyData;

    /// <summary>아군 데이터 설정 및 UI 초기화</summary>
    public void Initialize(CharacterData allyData)
    {
        _allyData = allyData;

        // 아이콘 설정
        if (_iconImage != null && allyData.icon != null)
        {
            _iconImage.sprite = allyData.icon;
            _iconImage.color = OrbColorMap.GetColor(allyData.attribute);
        }

        // 이름 설정
        if (_nameText != null)
        {
            _nameText.text = allyData.characterName;
        }
    }

    /// <summary>아군 데이터 반환</summary>
    public CharacterData GetData()
    {
        return _allyData;
    }
}