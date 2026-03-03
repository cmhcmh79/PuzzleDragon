// Assets/Scripts/Battle/AttributeSystem.cs
// 속성 상성을 계산하는 유틸리티 클래스

using UnityEngine;

public static class AttributeSystem
{
    /// <summary>
    /// 속성 상성 배수 계산
    /// 유리: 2배, 불리: 0.5배, 동일/중립: 1배
    /// </summary>
    public static float GetAttributeMultiplier(OrbType attackerAttr, OrbType defenderAttr)
    {
        // 회복 속성은 공격하지 않음
        if (attackerAttr == OrbType.Heal) return 0f;
        
        // 동일 속성
        if (attackerAttr == defenderAttr) return 1f;

        // 속성 상성표
        switch (attackerAttr)
        {
            case OrbType.Fire:
                if (defenderAttr == OrbType.Wood) return 2f;    // 불 > 나무
                if (defenderAttr == OrbType.Water) return 0.5f; // 불 < 물
                break;

            case OrbType.Water:
                if (defenderAttr == OrbType.Fire) return 2f;    // 물 > 불
                if (defenderAttr == OrbType.Wood) return 0.5f;  // 물 < 나무
                break;

            case OrbType.Wood:
                if (defenderAttr == OrbType.Water) return 2f;   // 나무 > 물
                if (defenderAttr == OrbType.Fire) return 0.5f;  // 나무 < 불
                break;

            case OrbType.Light:
                if (defenderAttr == OrbType.Dark) return 2f;    // 빛 > 어둠
                break;

            case OrbType.Dark:
                if (defenderAttr == OrbType.Light) return 2f;   // 어둠 > 빛
                break;
        }

        return 1f; // 기본
    }
}