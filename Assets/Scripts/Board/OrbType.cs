using UnityEngine;

public enum OrbType
{
    Fire,   // 불 - 빨강
    Water,  // 물 - 파랑
    Wood,   // 나무 - 초록
    Light,  // 빛 - 노랑
    Dark,   // 어둠 - 보라
    Heal    // 회복 - 분홍
}

/// <summary>
/// OrbType에 맞는 색상을 반환하는 유틸리티 클래스
/// </summary>
public static class OrbColorMap
{
    public static Color GetColor(OrbType type)
    {
        switch (type)
        {
            case OrbType.Fire:  return new Color(1.0f, 0.2f, 0.2f); // 빨강
            case OrbType.Water: return new Color(0.2f, 0.5f, 1.0f); // 파랑
            case OrbType.Wood:  return new Color(0.2f, 0.8f, 0.2f); // 초록
            case OrbType.Light: return new Color(1.0f, 0.9f, 0.2f); // 노랑
            case OrbType.Dark:  return new Color(0.6f, 0.2f, 0.9f); // 보라
            case OrbType.Heal:  return new Color(1.0f, 0.5f, 0.7f); // 분홍
            default:            return Color.white;
        }
    }
}