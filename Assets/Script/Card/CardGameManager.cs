using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance { get; private set; }

    // 정당 손패 참조
    public PartyCardHand playerHand; // 갑당
    public PartyCardHand partyBHand; // 을당
    public PartyCardHand partyCHand; // 병당

    // 턴 관리
    public enum Party { Player, PartyB, PartyC }
    public Party currentTurn;
    private bool isClockwise = true; // true: 갑→을→병, false: 병→을→갑

    // 공격 상태
    private bool isUnderAttack = false;
    private string attackCardId = null;
    private Party attacker;

    // 특수 효과
    private bool isAmplified = false; // S3 공격 증폭 효과

    // 턴 카운트 (종료 조건)
    public int playerActionCount = 0;
    public int partyBActionCount = 0;
    public int partyCActionCount = 0;

    // 현재 라운드
    public int currentRound = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 라운드 시작 (선공 랜덤 결정)
    public void StartRound()
    {
        // 턴 카운트 초기화
        playerActionCount = 0;
        partyBActionCount = 0;
        partyCActionCount = 0;

        // 공격 상태 초기화
        isUnderAttack = false;
        attackCardId = null;
        isAmplified = false;

        // 랜덤 선공 결정
        int random = Random.Range(0, 3);
        currentTurn = (Party)random;

        Debug.Log($"라운드 {currentRound} 시작! 선공: {GetPartyName(currentTurn)}");
    }

    // 다음 턴으로 진행
    public void NextTurn()
    {
        if (isClockwise)
        {
            // 갑 → 을 → 병
            if (currentTurn == Party.Player)
                currentTurn = Party.PartyB;
            else if (currentTurn == Party.PartyB)
                currentTurn = Party.PartyC;
            else
                currentTurn = Party.Player;
        }
        else
        {
            // 병 → 을 → 갑
            if (currentTurn == Party.Player)
                currentTurn = Party.PartyC;
            else if (currentTurn == Party.PartyC)
                currentTurn = Party.PartyB;
            else
                currentTurn = Party.Player;
        }
    }

    // 순서 반전 (S2 카드 효과)
    public void ReverseOrder()
    {
        isClockwise = !isClockwise;
        Debug.Log($"순서가 반전되었습니다! 현재 방향: {(isClockwise ? "갑→을→병" : "병→을→갑")}");
    }

    // 공격 카드 제시
    public void PlayAttackCard(string cardId, Party attackingParty)
    {
        isUnderAttack = true;
        attackCardId = cardId;
        attacker = attackingParty;

        Card card = CardDatabase.Instance.GetCard(cardId);
        Debug.Log($"{GetPartyName(attackingParty)}이(가) {card.cardName} 공격!");
    }

    // 방어 카드 제시 (완전 방어 성공 여부 반환)
    public bool PlayDefenseCard(string defenseCardId)
    {
        if (!isUnderAttack || attackCardId == null)
        {
            Debug.LogWarning("현재 공격받고 있지 않습니다.");
            return false;
        }

        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        Card defenseCard = CardDatabase.Instance.GetCard(defenseCardId);

        // 완전 방어 성공
        if (attackCard.fullDefenseCards.Contains(defenseCardId))
        {
            Debug.Log($"{defenseCard.cardName}(으)로 완전 방어 성공!");
            ClearAttackState();
            return true;
        }

        // 일부 방어 성공
        if (attackCard.partialDefenseCards.Contains(defenseCardId))
        {
            Debug.Log($"{defenseCard.cardName}(으)로 일부 방어 성공!");
            int damage = attackCard.attackValue / 3;
            if (isAmplified) damage *= 2;

            ApplyDamage(currentTurn, damage, attackCard.attackerGain / 3);
            ClearAttackState();
            return false;
        }

        Debug.LogWarning("방어 실패!");
        return false;
    }

    // S1: 공격 넘기기
    public void PassAttack()
    {
        if (!isUnderAttack)
        {
            Debug.LogWarning("현재 공격받고 있지 않습니다.");
            return;
        }

        Debug.Log($"{GetPartyName(currentTurn)}이(가) 공격을 다음 정당에게 넘겼습니다!");
        // 공격 상태는 유지하고 턴만 넘김
    }

    // S3: 공격 증폭
    public void AmplifyNextAttack()
    {
        isAmplified = true;
        Debug.Log("다음 공격이 2배로 증폭됩니다!");
    }

    // 방어 실패 시 피해 적용
    public void ApplyFullDamage()
    {
        if (!isUnderAttack || attackCardId == null)
            return;

        Card attackCard = CardDatabase.Instance.GetCard(attackCardId);
        int damage = attackCard.attackValue;
        if (isAmplified) damage *= 2;

        ApplyDamage(currentTurn, damage, attackCard.attackerGain);
        ClearAttackState();
    }

    // 지지율 변동 적용
    void ApplyDamage(Party defender, int damage, int attackerGain)
    {
        // 여기서 실제 지지율 변동 로직 구현
        // 전국 지지율 변경은 나중에 구현
        Debug.Log($"{GetPartyName(defender)} 지지율 -{damage}");
        Debug.Log($"{GetPartyName(attacker)} 지지율 +{attackerGain}");

        // 방어 실패 시 나머지 정당도 상승
        if (damage == CardDatabase.Instance.GetCard(attackCardId).attackValue)
        {
            int remainingGain = damage - attackerGain;
            Party otherParty = GetOtherParty(defender, attacker);
            Debug.Log($"{GetPartyName(otherParty)} 지지율 +{remainingGain}");
        }
    }

    // 공격 상태 초기화
    void ClearAttackState()
    {
        isUnderAttack = false;
        attackCardId = null;
        isAmplified = false; // 방어 성공 시에도 증폭 효과 소멸
    }

    // 턴 포기
    public void PassTurn()
    {
        IncrementActionCount(currentTurn);

        // 공격받고 있는 상태에서 턴 포기 시 전체 피해
        if (isUnderAttack)
        {
            ApplyFullDamage();
        }

        Debug.Log($"{GetPartyName(currentTurn)}이(가) 턴을 포기했습니다.");
    }

    // 행동 카운트 증가
    public void IncrementActionCount(Party party)
    {
        switch (party)
        {
            case Party.Player:
                playerActionCount++;
                break;
            case Party.PartyB:
                partyBActionCount++;
                break;
            case Party.PartyC:
                partyCActionCount++;
                break;
        }
    }

    // 종료 조건 체크
    public bool IsRoundEnd()
    {
        return playerActionCount >= 3 && partyBActionCount >= 3 && partyCActionCount >= 3;
    }

    // 헬퍼 메서드들
    public string GetPartyName(Party party)
    {
        switch (party)
        {
            case Party.Player: return "갑";
            case Party.PartyB: return "을";
            case Party.PartyC: return "병";
            default: return "알 수 없음";
        }
    }

    Party GetOtherParty(Party p1, Party p2)
    {
        if (p1 != Party.Player && p2 != Party.Player) return Party.Player;
        if (p1 != Party.PartyB && p2 != Party.PartyB) return Party.PartyB;
        return Party.PartyC;
    }

    public bool IsUnderAttack()
    {
        return isUnderAttack;
    }

    public string GetAttackCardId()
    {
        return attackCardId;
    }

    public bool IsClockwise()
    {
        return isClockwise;
    }
}