// Assets/Scripts/Battle/BattleManager.cs
// 전투 시스템 - 공격, 데미지 계산, 승리/패배 판정

using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private BoardCleaner _boardCleaner;

    /// <summary>구슬 매칭 완료 후 공격 처리</summary>
    public void ProcessAttack(Dictionary<OrbType, int> matchedOrbs, int comboCount)
    {
        Debug.Log("BattleManager - ProcessAttack()");

        List<AllyCard> allies = _gameUIManager.GetAllyCards();
        List<EnemySlot> enemies = _gameUIManager.GetAliveEnemies();

        if (enemies.Count == 0)
        {
            // 승리
            Debug.Log("승리!");
            return;
        }

        // 각 속성별로 공격 처리
        foreach (var pair in matchedOrbs)
        {
            OrbType orbType = pair.Key;
            int orbCount = pair.Value;

            // 회복 구슬 처리
            if (orbType == OrbType.Heal)
            {
                int healAmount = CalculateHealAmount(orbCount);
                _gameUIManager.Heal(healAmount);
                Debug.Log($"회복! {healAmount} HP");
                continue;
            }

            // 해당 속성 캐릭터들이 공격
            List<AllyCard> attackers = allies.FindAll(a => a.GetData().attribute == orbType);
            
            foreach (var attacker in attackers)
            {
                // 적 타겟 선택 (앞에서부터)
                EnemySlot target = GetNextTarget(enemies);
                if (target == null) break;

                // 데미지 계산
                int damage = CalculateDamage(attacker.GetData(), orbCount, comboCount, target.GetEnemyData());
                
                // 공격
                target.TakeDamage(damage);
                Debug.Log($"{attacker.GetData().characterName}이(가) {damage} 데미지!");

                // 적이 죽었으면 리스트에서 제거
                if (!target.IsAlive())
                {
                    Debug.Log($"적 처치!");
                    enemies.Remove(target);
                }
            }
        }

        // 적 턴 처리
        ProcessEnemyTurns();

        // 승리/패배 판정
        CheckGameOver();
    }

    // ============================================================
    // 데미지 계산
    // ============================================================

    /// <summary>
    /// 파드 공식: 기본공격력 × 드롭개수배수 × 콤보배수 × 속성상성
    /// </summary>
    private int CalculateDamage(CharacterData attacker, int orbCount, int comboCount, CharacterData defender)
    {
        // ① 기본 공격력
        float baseDamage = attacker.attack;

        // ② 드롭 개수 배수
        float orbMultiplier = GetOrbCountMultiplier(orbCount);

        // ③ 콤보 배수: 1 + (콤보수 - 1) × 0.25
        float comboMultiplier = 1f + (comboCount - 1) * 0.25f;

        // ④ 속성 상성
        float attributeMultiplier = AttributeSystem.GetAttributeMultiplier(attacker.attribute, defender.attribute);

        // 최종 데미지
        float totalDamage = baseDamage * orbMultiplier * comboMultiplier * attributeMultiplier;

        return Mathf.RoundToInt(totalDamage);
    }

    /// <summary>
    /// 드롭 개수 배수
    /// 3개=1배, 4개=1.25배, 5개=1.5배, 6개=1.75배, 이후 +0.25
    /// </summary>
    private float GetOrbCountMultiplier(int orbCount)
    {
        if (orbCount <= 3) return 1f;
        if (orbCount == 4) return 1.25f;
        if (orbCount == 5) return 1.5f;
        if (orbCount == 6) return 1.75f;
        
        // 7개 이상: 1.75 + (개수 - 6) * 0.25
        return 1.75f + (orbCount - 6) * 0.25f;
    }

    /// <summary>회복량 계산 (구슬 개수별)</summary>
    private int CalculateHealAmount(int orbCount)
    {
        if (orbCount == 3) return 100;
        if (orbCount == 4) return 150;
        if (orbCount >= 5) return 200;
        return 100;
    }

    // ============================================================
    // 타겟팅
    // ============================================================

    /// <summary>다음 타겟 선택 (앞에서부터)</summary>
    private EnemySlot GetNextTarget(List<EnemySlot> enemies)
    {
        if (enemies.Count == 0) return null;
        return enemies[0];
    }

    // ============================================================
    // 적 턴 처리
    // ============================================================

    /// <summary>적 턴 감소 및 공격</summary>
    private void ProcessEnemyTurns()
    {
        List<EnemySlot> enemies = _gameUIManager.GetAliveEnemies();

        foreach (var enemy in enemies)
        {
            enemy.DecreaseTurn();

            // 턴이 0이면 공격
            int damage = enemy.Attack();
            if (damage > 0)
            {
                _gameUIManager.TakeDamage(damage);
                Debug.Log($"적 공격! {damage} 데미지 받음");
            }
        }
    }

    // ============================================================
    // 승리/패배 판정
    // ============================================================

    private void CheckGameOver()
    {
        // 패배 판정
        if (_gameUIManager.GetCurrentHP() <= 0)
        {
            Debug.Log("패배...");
            Time.timeScale = 0; // 게임 정지
            return;
        }

        // 승리 판정
        if (_gameUIManager.GetAliveEnemies().Count == 0)
        {
            Debug.Log("승리!");
            Time.timeScale = 0; // 게임 정지
            return;
        }
    }
}